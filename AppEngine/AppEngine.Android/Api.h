#pragma once

#include "../AppEngine.Shared/Api.h"

extern "C" {
    int GetBidFromRule(Phase phase, const char* hand, int lastBidId, int position,
        int* minSuitsPartner, int* minSuitsOpener, Phase* newPhase, char* description);
    void GetRulesByBid(Phase phase, int bidId, int position, char* information);
    int Setup(const char* database);
}