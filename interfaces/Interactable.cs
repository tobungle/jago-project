
public interface Interactable
{
	public enum InteractableType
	{
		Normal,
		Item
	}
	InteractableType GetInteractableType();
	void Interact();
	string GetInteractPrompt();
}
