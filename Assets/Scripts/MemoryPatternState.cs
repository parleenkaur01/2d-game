using UnityEngine;

public enum MemoryResult { Wrong, Accepted, RoundComplete, Solved }

public class MemoryPatternState
{
    private readonly int[][] patterns;
    public int Round { get; private set; }
    public int InputPosition { get; private set; }
    public bool Solved => Round == patterns.Length;
    public int[] CurrentPattern => patterns[Mathf.Min(Round, patterns.Length - 1)];

    public MemoryPatternState(int[][] sequences) => patterns = sequences;

    public static MemoryPatternState CreateRandom()
    {
        int[][] sequences = new int[3][];
        for (int round = 0; round < sequences.Length; round++)
        {
            int[] objects = { 0, 1, 2, 3, 4, 5 };
            for (int i = objects.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int swap = objects[i]; objects[i] = objects[j]; objects[j] = swap;
            }
            sequences[round] = new int[round + 2];
            for (int i = 0; i < sequences[round].Length; i++) sequences[round][i] = objects[i];
        }
        return new MemoryPatternState(sequences);
    }

    public void ResetAttempt() => InputPosition = 0;

    public MemoryResult Press(int index)
    {
        if (Solved) return MemoryResult.Solved;
        if (CurrentPattern[InputPosition] != index)
        {
            InputPosition = 0;
            return MemoryResult.Wrong;
        }
        InputPosition++;
        if (InputPosition < CurrentPattern.Length) return MemoryResult.Accepted;
        InputPosition = 0;
        Round++;
        return Solved ? MemoryResult.Solved : MemoryResult.RoundComplete;
    }
}
