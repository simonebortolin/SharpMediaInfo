using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpMediaInfo.GUI.ViewModels
{
    public interface ITabViewModel
    {
        string Id { get; }
        string Header { get; }

        Task<bool> OnCloseAsync();
    }
}
