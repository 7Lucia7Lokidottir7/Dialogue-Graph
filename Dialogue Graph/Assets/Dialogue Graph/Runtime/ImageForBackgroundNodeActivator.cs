using PG.DialogueGraph;
using UnityEngine;
using UnityEngine.UI;

public class ImageForBackgroundNodeActivator : MonoBehaviour
{
    [SerializeField] private DialogueManager _manager;
    [SerializeField] private Image _image;
    private void OnEnable()
    {
        _manager.dialogueChanged += UpdateImage;
    }
    private void OnDisable()
    {
        _manager.dialogueChanged -= UpdateImage;
    }
    // Update is called once per frame
    void UpdateImage(RuntimeDialogueNode runtimeDialogueNode)
    {
        if (runtimeDialogueNode.background != null)
        {
            _image.sprite = runtimeDialogueNode.background;
        }
    }
}
