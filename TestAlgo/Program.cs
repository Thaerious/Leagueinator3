using Algorithms;
using Algorithms.PairMatching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utility.Extensions;

string csvBowls = @"
Alice,Rosa,26
Alice,Jack,1
Alice,Gemma,90
Alice,Clara,8
Alice,Tessa,62
Alice,Sam,66
Alice,Eva,50
Alice,Quinn,96
Ben,Ivy,37
Ben,Kate,20
Ben,Uma,88
Ben,Clara,52
Ben,Gemma,24
Ben,Isla,65
Ben,Zane,38
Ben,Peter,57
Chloe,Jack,24
Chloe,Leo,72
Chloe,Felix,18
Chloe,Mia,51
Chloe,Lara,25
Chloe,Noah,72
Chloe,Alice,54
Chloe,Wendy,42
Daniel,Quinn,29
Daniel,Nate,63
Daniel,Rosa,69
Daniel,Opal,88
Daniel,Lara,48
Daniel,Hugo,33
Daniel,Kate,47
Daniel,Owen,62
Eva,Hugo,33
Eva,Chloe,29
Eva,Daniel,29
Eva,Sam,65
Eva,Ben,42
Eva,Peter,64
Eva,Uma,9
Eva,Rosa,42
Felix,Jack,42
Felix,Owen,55
Felix,Nate,92
Felix,Gemma,42
Felix,Tessa,24
Felix,Sam,12
Felix,Yara,18
Felix,Clara,85
Gemma,Kate,11
Gemma,Opal,64
Gemma,Peter,10
Gemma,Daniel,63
Gemma,Eva,63
Gemma,Yara,85
Gemma,Hugo,38
Gemma,Jack,64
Hugo,Opal,88
Hugo,Ivy,89
Hugo,Zane,28
Hugo,Tessa,63
Hugo,Quinn,9
Hugo,Noah,54
Hugo,Rosa,22
Hugo,Kate,10
Isla,Victor,21
Isla,Yara,70
Isla,Grace,11
Isla,Sam,40
Isla,Lara,28
Isla,Noah,39
Isla,Peter,96
Isla,Xander,70
Jack,Xander,53
Jack,Nate,53
Jack,Felix,79
Jack,Opal,45
Jack,Owen,99
Jack,Isla,67
Jack,Mia,33
Jack,Ben,64
Kate,Grace,55
Kate,Uma,85
Kate,Mia,17
Kate,Peter,42
Kate,Zane,47
Kate,Noah,61
Kate,Lara,87
Kate,Yara,8
Leo,Zane,41
Leo,Xander,72
Leo,Opal,52
Leo,Ben,26
Leo,Uma,99
Leo,Hugo,75
Leo,Kate,72
Leo,Felix,8
Mia,Leo,43
Mia,Daniel,60
Mia,Felix,7
Mia,Wendy,13
Mia,Clara,96
Mia,Ben,87
Mia,Peter,80
Mia,Grace,44
Nate,Isla,90
Nate,Eva,40
Nate,Yara,53
Nate,Tessa,24
Nate,Victor,19
Nate,Kate,10
Nate,Mia,8
Nate,Xander,93
Opal,Ben,91
Opal,Felix,44
Opal,Tessa,3
Opal,Peter,8
Opal,Kate,45
Opal,Leo,24
Opal,Quinn,99
Opal,Yara,14
Peter,Nate,16
Peter,Ben,55
Peter,Alice,89
Peter,Quinn,31
Peter,Uma,68
Peter,Clara,76
Peter,Daniel,58
Peter,Kate,75
Quinn,Daniel,46
Quinn,Ivy,26
Quinn,Lara,15
Quinn,Peter,45
Quinn,Owen,16
Quinn,Mia,47
Quinn,Victor,40
Quinn,Opal,96
Rosa,Opal,20
Rosa,Grace,18
Rosa,Daniel,49
Rosa,Yara,52
Rosa,Lara,34
Rosa,Isla,29
Rosa,Alice,49
Rosa,Peter,43
Sam,Quinn,14
Sam,Uma,69
Sam,Peter,49
Sam,Yara,96
Sam,Opal,87
Sam,Rosa,12
Sam,Eva,28
Sam,Isla,65
Tessa,Wendy,13
Tessa,Daniel,37
Tessa,Quinn,67
Tessa,Yara,67
Tessa,Leo,95
Tessa,Felix,2
Tessa,Mia,43
Tessa,Noah,21
Uma,Chloe,29
Uma,Opal,4
Uma,Eva,93
Uma,Yara,62
Uma,Felix,80
Uma,Mia,70
Uma,Zane,12
Uma,Isla,78
Victor,Uma,17
Victor,Rosa,36
Victor,Peter,26
Victor,Ivy,62
Victor,Quinn,53
Victor,Yara,62
Victor,Eva,98
Victor,Wendy,31
Wendy,Ivy,3
Wendy,Leo,72
Wendy,Noah,17
Wendy,Felix,73
Wendy,Isla,4
Wendy,Mia,29
Wendy,Yara,39
Wendy,Lara,36
Xander,Owen,94
Xander,Sam,80
Xander,Peter,21
Xander,Eva,18
Xander,Felix,6
Xander,Quinn,2
Xander,Nate,45
Xander,Isla,92
Yara,Owen,76
Yara,Daniel,9
Yara,Noah,27
Yara,Gemma,81
Yara,Quinn,100
Yara,Kate,85
Yara,Opal,64
Yara,Rosa,62
Zane,Hugo,13
Zane,Clara,72
Zane,Mia,89
Zane,Sam,27
Zane,Ivy,71
Zane,Daniel,34
Zane,Quinn,22
Zane,Isla,72
Ivy,Ben,66
Ivy,Daniel,67
Ivy,Yara,14
Ivy,Sam,40
Ivy,Noah,81
Ivy,Uma,73
Ivy,Quinn,47
Ivy,Grace,67
Owen,Quinn,36
Owen,Alice,45
Owen,Daniel,9
Owen,Opal,34
Owen,Noah,91
Owen,Jack,41
Owen,Felix,98
Owen,Eva,19
Lara,Felix,5
Lara,Gemma,37
Lara,Mia,100
Lara,Opal,11
Lara,Leo,59
Lara,Jack,63
Lara,Grace,58
Lara,Yara,62
Noah,Victor,96
Noah,Grace,3
Noah,Gemma,44
Noah,Nate,63
Noah,Opal,7
Noah,Eva,65
Noah,Quinn,2
Noah,Mia,78
Grace,Gemma,54
Grace,Eva,88
Grace,Kate,88
Grace,Felix,26
Grace,Zane,18
Grace,Victor,83
Grace,Nate,79
Grace,Noah,45
Clara,Sam,77
Clara,Lara,33
Clara,Gemma,29
Clara,Kate,89
Clara,Chloe,80
Clara,Jack,98
Clara,Victor,91
Clara,Alice,82

";

string csvAlphabet = @"
A,B,1
A,D,1
B,C,1
D,E,1
C,F,1
";

Graph<string> graph = Graph<string>.FromCSV(csvBowls);
//BruteForce();
//Naive();
//GreedyAugmenting();
Single();

void Single() {
    Debug.WriteLine("Single Solution");
    Random rng = new Random(234234);
    Solution<string> solution = new(graph);
    solution.Set(["Felix", "Grace", "Kate", "Zane", "Victor", "Peter", "Daniel", "Opal", "Wendy", "Tessa", "Mia", "Quinn", "Nate", "Xander", "Leo", "Uma", "Noah", "Eva", "Alice", "Jack", "Yara", "Sam", "Hugo", "Rosa", "Chloe", "Lara", "Ben", "Gemma"]);
    Debug.WriteLine(solution);
    var repaired = new ImproveSolution<string>(graph, solution).Repair();
    Debug.WriteLine($"\nRepaired:\n{repaired}");
}

void Greedy() {
    const int POPSZ = 100;
    const int GENCNT = 50;

    Debug.WriteLine("Greedy Solution");
    Random rng = new Random(123654789);
    List<Solution<string>> population = [];

    // Build initial solutions from randomly selected initialEdges.
    while (population.Count < POPSZ) {
        Solution<string> solution = new(graph);
        var initialEdges = graph.Edges.OrderBy(edge => rng.Next()).ToList();
        foreach (var edge in initialEdges) {
            if (solution.Find(edge.Item1) != -1) continue;
            if (solution.Find(edge.Item2) != -1) continue;
            solution.Set(edge);
        }
        Debug.WriteLine(solution.ToList().JoinString(", ", "\""));
        var repaired = new RepairSolution<string>(graph, solution).Repair();
        population.Add(repaired);
    }

    PrintBest(population);

    var edges = graph.Edges.OrderBy(edge => rng.Next()).ToList();
    int bestGen = 0;
    int bestFit = 0;

    for (int i = 0; i < GENCNT; i++) {
        List<Solution<string>> next = [];

        foreach (var solution in population) {
            var copy = solution.Copy();
            var edge = edges[rng.Next(population.Count)];
            copy.Set(edge);
            var repaired = new RepairSolution<string>(graph, copy).Repair();
            next.Add(repaired);
        }

        population.AddRange(next);
        population = population.OrderBy(s => s.Fitness).Reverse().Take(POPSZ).ToList();
        Debug.WriteLine(population[0]);

        if (population[0].Fitness > bestFit) {
            bestFit = population[0].Fitness;
            bestGen = i;
        }
    }
    Debug.WriteLine($"best fitness '{bestFit}' found on generation '{bestGen}'.");
}

void PrintBest(List<Solution<string>> population) {
    var best = population.OrderBy(s => s.Fitness).Reverse().First();
    Debug.WriteLine($"best");
}


void Naive() {
    Debug.WriteLine("Naive Solution");
    Solution<string> solution = new(graph);
    var repaired = new RepairSolution<string>(graph, solution).Repair();
    Stopwatch sw = Stopwatch.StartNew();
    Debug.WriteLine(repaired);
    sw.Stop();
    Debug.WriteLine($"{sw.ElapsedMilliseconds} ms\n");
}

void BruteForce() {
    Debug.WriteLine("Brute Force Solution");
    BruteForce<string> bruteForce = new(graph);
    Stopwatch sw = Stopwatch.StartNew();
    List<Solution<string>> best = [];
    foreach (Solution<string> solution in bruteForce.Run()) {
        best.Add(solution);
        if (best.Count == 100000) break;
    }

    best = best.OrderBy(s => s.Fitness).ToList();

    Debug.WriteLine(best[^1]);

    sw.Stop();
    Debug.WriteLine($"{sw.ElapsedMilliseconds} ms\n");
}
