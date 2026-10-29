using System.Collections.Generic;
using UnityEngine;

/// <summary>Letter-tile rules, independent of the room and interface.</summary>
public class WordScrambleState
{
    private static readonly string[] Words = { "TIME", "FLOOR", "ESCAPE" };
    private static readonly string[] Clues = {
        "Clocks measure this, but can never stop it.",
        "An elevator stops on one of these.",
        "What you hope to do when the final door opens."
    };
    private readonly List<int> selection = new List<int>();
    public char[] Tiles { get; private set; }
    public int SolvedCount { get; private set; }
    public bool Solved => SolvedCount == Words.Length;
    public string Clue => Clues[Mathf.Min(SolvedCount, Words.Length - 1)];
    public string FirstLetter => Words[Mathf.Min(SolvedCount, Words.Length - 1)].Substring(0, 1);
    public string Answer
    {
        get
        {
            char[] letters = new char[selection.Count];
            for (int i = 0; i < selection.Count; i++) letters[i] = Tiles[selection[i]];
            return new string(letters);
        }
    }

    public WordScrambleState() => Shuffle();
    public bool IsUsed(int index) => selection.Contains(index);

    public bool Choose(int index)
    {
        if (Solved || index < 0 || index >= Tiles.Length || IsUsed(index)) return false;
        selection.Add(index);
        return true;
    }

    public bool ChooseLetter(char letter)
    {
        for (int i = 0; i < Tiles.Length; i++)
            if (Tiles[i] == char.ToUpperInvariant(letter) && Choose(i)) return true;
        return false;
    }

    public void Undo()
    {
        if (selection.Count > 0) selection.RemoveAt(selection.Count - 1);
    }

    public void RemoveAt(int slot)
    {
        if (slot >= 0 && slot < selection.Count) selection.RemoveAt(slot);
    }

    public void Clear() => selection.Clear();

    public void Shuffle()
    {
        if (Solved) return;
        selection.Clear();
        string word = Words[SolvedCount];
        Tiles = word.ToCharArray();
        for (int i = Tiles.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            char swap = Tiles[i]; Tiles[i] = Tiles[j]; Tiles[j] = swap;
        }
        if (new string(Tiles) == word)
        {
            char swap = Tiles[0]; Tiles[0] = Tiles[1]; Tiles[1] = swap;
        }
    }

    public bool Submit()
    {
        if (Solved || Answer != Words[SolvedCount]) return false;
        SolvedCount++;
        if (!Solved) Shuffle();
        return true;
    }
}
