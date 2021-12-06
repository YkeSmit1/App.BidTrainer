#pragma once

#include "../AppEngine.Shared/Api.h"

extern "C" {
    int GetBidFromRule(Phase phase, const char* hand, int lastBidId, int position, int* minSuitsPartner, int* minSuitsOpener,
        const char* previousBidding, const char* previousSlamBidding, bool isCompetitive, int minHcpPartner, bool allControlsPresent, Phase* newPhase, char* description);
    void GetRulesByBid(Phase phase, int bidId, int position, const char* previousBidding, bool isCompetitive, char* information);
    void GetRelativeRulesByBid(int bidId, const char* previousBidding, char* information);
    int Setup(const char* database);
    void SetModules(int modules);
}