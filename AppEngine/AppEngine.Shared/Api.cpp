#include "Api.h"

#include <string>
#include <iostream>
#include <sstream>
#include <fstream>
#include "Rule.h"
#include "SQLiteCppWrapper.h"
#include "Utils.h"
#include "BoardCharacteristic.h"
#include <android/log.h>

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
    __android_log_print(ANDROID_LOG_INFO, "BidTrainer", "GetSqliteWrapper");
    static std::unique_ptr<ISQLiteWrapper> sqliteWrapper = std::make_unique<SQLiteCppWrapper>("/data/user/0/app.bidtrainer/files/four_card_majors.db3");
    return sqliteWrapper.get();
}

int GetBidFromRule(Phase phase, const char* hand, int lastBidId, int position, int* minSuitsPartner, int* minSuitsOpener, 
    const char* previousBidding, const char* previousSlamBidding, bool isCompetitive, int minHcpPartner, bool allControlsPresent, Phase* newPhase, char* description)
{
    auto handCharacteristic = GetHandCharacteristic(hand);
    auto minSuitsPartnerVec = std::vector<int>(minSuitsPartner, minSuitsPartner + 4);
    auto opponentsSuits = std::vector<int>(minSuitsOpener, minSuitsOpener + 4);
    auto boardCharacteristic = BoardCharacteristic::Create(handCharacteristic, minSuitsPartnerVec, opponentsSuits);

    auto [bidId, lNewfase, descr] = minHcpPartner + Utils::CalculateHcp(hand) < 29 && phase != Phase::SlamBidding ?
        GetSqliteWrapper()->GetRule(handCharacteristic, boardCharacteristic, phase, lastBidId, position, previousBidding, isCompetitive) :
        GetSqliteWrapper()->GetRelativeRule(handCharacteristic, boardCharacteristic, lastBidId, previousSlamBidding, allControlsPresent);
    assert(descr.size() < 128);
    strncpy(description, descr.c_str(), descr.size());
    description[descr.size()] = '\0';
    *newPhase = lNewfase;
    return bidId;
}

int Setup(const char* database)
{
    GetSqliteWrapper()->SetDatabase(database);
    return 0;
}

void GetBid(int bidId, int& rank, int& suit)
{
    GetSqliteWrapper()->GetBid(bidId, rank, suit);
}

void GetRulesByBid(Phase phase, int bidId, int position, const char* previousBidding, bool isCompetitive, char* information)
{
    auto linformation = GetSqliteWrapper()->GetRulesByBid(phase, bidId, position, previousBidding, isCompetitive);
    assert(linformation.size() < 8192);
    strncpy(information, linformation.c_str(), linformation.size());
    information[linformation.size()] = '\0';
}

void GetRelativeRulesByBid(int bidId, const char* previousBidding, char* information)
{
    auto linformation = GetSqliteWrapper()->GetRelativeRulesByBid(bidId, previousBidding);
    assert(linformation.size() < 8192);
    strncpy(information, linformation.c_str(), linformation.size());
    information[linformation.size()] = '\0';
}

void SetModules(int modules)
{
    GetSqliteWrapper()->SetModules(modules);
}