using System;
using UnityEngine;

[Serializable]
public class CharacterDTO
{
    public int id = -1;
    public string name = "empty";
    public int level = 1;
    public int money = 0;
    
    public int hair = 0;
    public int helmet = 0;
    public int beard = 0;
    public int armor = 0;
    public int pants = 0;
    public int weapon = 0;
    public int back = 0;
}

[System.Serializable]
public class MatchDTO
{
    public CharacterDTO player_character;
    public CharacterDTO enemy_character;
    public bool is_starting;
}

[System.Serializable]
public class CharacterID
{
    public int character_id;
}
