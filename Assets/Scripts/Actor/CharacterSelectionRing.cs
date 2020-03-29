using UnityEngine;

public class CharacterSelectionRing : MonoBehaviour
{
    private Character _character;

    public void OnMouseDown()
    {
        if(_character == null)
        {
            _character = transform.parent.GetComponent<Character>();
            if (_character == null)
                Logger.Error("Could not find character for selection ring");
        }

        _character.PlayerLocomotion.SetInputListeningFreeze(10);
        CharacterManager.Instance.SelectCharacter(_character);
    }


}
