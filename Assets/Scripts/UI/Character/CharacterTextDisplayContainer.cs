using System.Collections.Generic;
using UnityEngine;

public class CharacterTextDisplayContainer : MonoBehaviour
{
    public static CharacterTextDisplayContainer Instance;

    public Dictionary<Character, CharacterRoutineText> CharacterRoutineTexts = new Dictionary<Character, CharacterRoutineText>();
    public GameObject CharacterTextDisplayPrefab;

    public void Awake()
    {
        Instance = this;

        Guard.CheckIsNull(CharacterTextDisplayPrefab, "CharacterTextDisplayPrefab");
    }

    private void AddCharacterRoutineText(Character character, CharacterRoutineText characterRoutineText)
    {
        CharacterRoutineTexts.Add(character, characterRoutineText);
    }

    public void EnableCharacterRoutineText(Character character, string text)
    {
        if(!CharacterRoutineTexts.ContainsKey(character))
        {
            CharacterRoutineText characterRoutineText = Instantiate(CharacterTextDisplayPrefab, transform).GetComponent<CharacterRoutineText>();
            characterRoutineText.Initialise(character);
            AddCharacterRoutineText(character, characterRoutineText);
        }

        CharacterRoutineTexts[character].SetText(text);
        CharacterRoutineTexts[character].Enable();
    }

    public void DisableCharacterRoutineText(Character character)
    {
        if (!CharacterRoutineTexts.ContainsKey(character))
        {
            Logger.Error("Cannot find character routine text ui element for {0}", character.FullName());
        }
        
        CharacterRoutineTexts[character].Disable();
    }


    // only remove when character gets deleted, otherwise hide go
    public void RemoveCharacterRoutineText(Character character)
    {
        // needs null check
        CharacterRoutineText characterRoutineText = CharacterRoutineTexts[character];
        CharacterRoutineTexts.Remove(character);
        characterRoutineText.Delete();
    }
}
