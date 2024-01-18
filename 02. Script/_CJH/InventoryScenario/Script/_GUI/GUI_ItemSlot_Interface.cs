using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GUI_ItemSlot_Interface
{
    public GUI_ItemUnit get_GUI_ItemUnit();
    public void ApplyDataToGridGUI(GUI_ItemUnit iu);
}
