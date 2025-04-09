using UnityEngine;

namespace Infrastructure.UIPanels.Inventory.DraggedHelper
{
	public interface IDragCardIntermediary
	{
		//public SessionWeaponConfiguration SessionWeaponConfiguration { get; set; }
		public Transform Transform { get; set; }
		public DraggedState DraggedState { get; set; }
	}
}