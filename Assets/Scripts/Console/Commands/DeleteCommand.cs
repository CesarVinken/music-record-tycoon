using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DeleteCommand : CommandProcedure
{
    public override void Run(List<string> arguments)
    {
        if (BuilderManager.Instance == null)
        {
            Console.Instance.PrintToReportText("The delete command can only be used while in game");
            return;
        }
        switch (arguments[0])
        {
            //case "room":
            //    await DeleteRoom(arguments);
            //    break;
            case "character":
                DeleteCharacter(arguments);
                break;
            //case "characters":
            //    await DeleteCharacters(arguments);
            //    break;
            default:
                Console.Instance.PrintToReportText("Unknown delete command to delete " + arguments[0]);
                break;
        }
    }

    public void DeleteCharacter(List<string> allArguments)
    {
        List<string> arguments = allArguments.Where((v, i) => i != 0).ToList();
        if(arguments.Count == 0)
        {
            Console.Instance.PrintToReportText("Need more arguments to specify which character to delete. Add name, first-name, last-name or id.");
            return;
        }
        Character characterToDelete;
        switch (arguments[0])
        {
            case "name":
                if(arguments.Count < 2)
                {
                    Console.Instance.PrintToReportText("In order to delete a character by name, you need to submit a full name with spaces replaced by dashes (eg. 'Michael-Jackson')");
                    return;
                }
                characterToDelete = FindCharacter(arguments[1].Replace('-', ' '), NameType.FullName);

                if (characterToDelete == null)
                {
                    Console.Instance.PrintToReportText("Could not find character with name " + arguments[1]);
                    return;
                }

                CharacterManager.Instance.DeleteCharacter(characterToDelete);

                return;
            case "first-name":
                if (arguments.Count < 2)
                {
                    Console.Instance.PrintToReportText("In order to delete a character by first name, you need to submit a first name with spaces replaced by dashes (eg. 'Franz-Joseph')");
                    return;
                }
                characterToDelete = FindCharacter(arguments[1].Replace('-', ' '), NameType.FirstName);

                if (characterToDelete == null)
                {
                    Console.Instance.PrintToReportText("Could not find character with name " + arguments[1]);
                    return;
                }

                CharacterManager.Instance.DeleteCharacter(characterToDelete);

                break;
            case "last-name":
                if (arguments.Count < 2)
                {
                    Console.Instance.PrintToReportText("In order to delete a character by last name, you need to submit a first name with spaces replaced by dashes (eg. 'Lloyd-Webber')");
                    return;
                }
                characterToDelete = FindCharacter(arguments[1].Replace('-', ' '), NameType.LastName);

                if (characterToDelete == null)
                {
                    Console.Instance.PrintToReportText("Could not find character with name " + arguments[1]);
                    return;
                }

                CharacterManager.Instance.DeleteCharacter(characterToDelete);

                break;
            case "id":
                if (arguments.Count < 2)
                {
                    Console.Instance.PrintToReportText("In order to delete a character by id, you need to submit the id number of the character, which is a uuid (eg. 'Lloyd-Webber')");
                    return;
                }

                characterToDelete = CharacterManager.Instance.Characters.Find(i => i.Id == arguments[1]);
                if (characterToDelete == null)
                {
                    Console.Instance.PrintToReportText("Could not find character with id " + arguments[1]);
                    return;
                }

                CharacterManager.Instance.DeleteCharacter(characterToDelete);

                break;
            default:
                Console.Instance.PrintToReportText("Unknown argument " + arguments[0] + ". It is possible to delete a character by name, first-name, last-name or id");
                return;
        }
        // delete by id
    }

    private Character FindCharacter(string satanisedName, NameType nameType)
    {
        List<Character> charactersInGame = CharacterManager.Instance.Characters;

        Character characterToDelete = null;
        Logger.Log("Trying to find {0}", satanisedName);
        if(nameType == NameType.FullName)
            characterToDelete = charactersInGame.Find(i => CharacterNameGenerator.GetName(i.Name).ToLower() == satanisedName);
        if (characterToDelete == null && (nameType == NameType.FirstName || nameType == NameType.FullName))
            characterToDelete = charactersInGame.Find(i => i.Name.FirstName.ToLower() == satanisedName);
        if (characterToDelete == null && (nameType == NameType.LastName || nameType == NameType.FullName))
            characterToDelete = charactersInGame.Find(i => i.Name.LastName.ToLower() == satanisedName);

        return characterToDelete;
    }

    private enum NameType {
        FirstName,
        LastName,
        FullName
    }
}
