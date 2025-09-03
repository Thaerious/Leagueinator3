using Leagueinator.GUI.Model;
using System.Diagnostics;

string mockFile = @"
Controller Data
File Name: Leagueinator
Is Saved: True
Current Round Index: 1 / 1

No. of Events: 1
Event Name: August 28 2025
Number of Lanes: 8
Default Ends: 10
Match Format: VS2
Event Type: Swiss
Match Scoring: Plus
No. of Rounds: 1
Round 0:
Match 0: | MatchFormat: VS2 | Players: [[adam, eve][cain, able]] | Score: [0, 0] | TB: -1 | Ends: 10
Match 1: | MatchFormat: VS2 | Players: [[,][,]] | Score: [0, 0] | TB: -1 | Ends: 10
Match 2: | MatchFormat: VS2 | Players: [[,][,]] | Score: [0, 0] | TB: -1 | Ends: 10
Match 3: | MatchFormat: VS2 | Players: [[,][,]] | Score: [0, 0] | TB: -1 | Ends: 10
Match 4: | MatchFormat: VS2 | Players: [[,][,]] | Score: [0, 0] | TB: -1 | Ends: 10
Match 5: | MatchFormat: VS2 | Players: [[,][,]] | Score: [0, 0] | TB: -1 | Ends: 10
Match 6: | MatchFormat: VS2 | Players: [[,][,]] | Score: [0, 0] | TB: -1 | Ends: 10
Match 7: | MatchFormat: VS2 | Players: [[,][,]] | Score: [0, 0] | TB: -1 | Ends: 11
";

LeagueData leagueData = new Loader().Load(mockFile);
Debug.WriteLine(leagueData.ToString());


