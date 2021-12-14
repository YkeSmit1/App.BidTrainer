#pragma once

enum class Phase
{
    Unknown,
    Opening,
    OneSuit,
    OneNT,
    Stayman,
    JacobyHearts,
    JacobySpades,
    OverCall,
    TakeOutDbl,
    OneNTOvercall
};

enum class BidKind
{
    UnknownSuit,
    FirstSuit,
    SecondSuit,
    LowestSuit,
    HighestSuit,
    PartnersSuit,
    OpponentsSuit
};

extern "C" {
    int GetBidFromRule(Phase phase, const char* hand, int lastBidId, int position,
        int* minSuitsPartner, int* minSuitsOpener, Phase* newPhase, char* description);
    void GetRulesByBid(Phase phase, int bidId, int position, char* information);
    int Setup(const char* database);
}