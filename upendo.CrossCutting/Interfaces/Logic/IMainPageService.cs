using System;
using System.Collections.Generic;
using upendo.CrossCutting.Entities;

namespace upendo.CrossCutting.Interfaces.Logic
{
    public interface IMainPageService
    {
        IEnumerable<MenuOption> GetMenuOptions(bool darkTheme = false);
    }
}
