#include "Api.h"

#include <string>
#include "Rule.h"
#include "SQLiteCppWrapper.h"
#include "BoardCharacteristic.h"
#include "InformationFromAuction.h"

std::unique_ptr<ISQLiteWrapper> sqliteWrapper = nullptr;


HandCharacteristic GetHandCharacteristic(const std::string& hand)
{
    static HandCharacteristic handCharacteristic{};
    if (hand != handCharacteristic.hand)
    {
        handCharacteristic.Initialize(hand);
    }
    return handCharacteristic;
}

ISQLiteWrapper* GetSqliteWrapper()
{
    if (sqliteWrapper == nullptr)
        throw std::logic_error("Setup was not called to initialize sqlite database");
    return sqliteWrapper.get();
}

int GetBidFromRule(const char* hand, const char* previousBidding, char* description)
{    
    auto handCharacteristic = GetHandCharacteristic(hand);
    InformationFromAuction informationFromAuction{ GetSqliteWrapper(), previousBidding};
    BoardCharacteristic boardCharacteristic{ handCharacteristic, previousBidding, informationFromAuction };

    auto isSlambidding = informationFromAuction.isSlamBidding || ((handCharacteristic.Hcp + boardCharacteristic.minHcpPartner >= 29 && boardCharacteristic.hasFit));

    auto [bidId, descr] = !isSlambidding ?
        GetSqliteWrapper()->GetRule(handCharacteristic, boardCharacteristic, previousBidding) :
        GetSqliteWrapper()->GetRelativeRule(handCharacteristic, boardCharacteristic, informationFromAuction.previousSlamBidding);
    assert(descr.size() < 128);
    strcpy(description, descr.c_str());
    return bidId;
}

int Setup(const char* database)
{
    sqliteWrapper = std::make_unique<SQLiteCppWrapper>(database);
    return 0;
}

void GetRulesByBid(int bidId, const char* previousBidding, char* information)
{
    InformationFromAuction informationFromAuction{ GetSqliteWrapper(), previousBidding };
    std::string linformation;
    if (informationFromAuction.isSlamBidding)
        linformation = GetSqliteWrapper()->GetRelativeRulesByBid(bidId, informationFromAuction.previousSlamBidding);
    else
        linformation = GetSqliteWrapper()->GetRulesByBid(bidId, previousBidding);
    assert(linformation.size() < 8192);
    strcpy(information, linformation.c_str());
}

void SetModules(int modules)
{
    GetSqliteWrapper()->SetModules(modules);
}

void GetInformationFromAuction(const char* previousBidding, char* informationFromAuctionjson)
{
    InformationFromAuction informationFromAuction{ GetSqliteWrapper(), previousBidding };
    auto json = informationFromAuction.AsJson();
    assert(json.size() < 8192);
    strcpy(informationFromAuctionjson, json.c_str());
}
