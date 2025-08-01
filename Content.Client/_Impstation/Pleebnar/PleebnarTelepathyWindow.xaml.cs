using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared._Impstation.Pleebnar;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client._Impstation.Pleebnar;

[GenerateTypedNameReferences]
public sealed partial class PleebnarTelepathyWindow : FancyWindow
{
    [Dependency] private readonly IPrototypeManager _proto = default!;
    public Action<string?>? OnVisionSelect;
    private ProtoId<PleebnarVisionPrototype> _vision;
    private readonly List<PleebnarVisionPrototype> _visions = [];
    private string _searchText = string.Empty;
    // init after opening
    public PleebnarTelepathyWindow()
    {
        IoCManager.InjectDependencies(this);
        RobustXamlLoader.Load(this);
        SearchBar.OnTextChanged += OnSearchTextChanged;

    }
    //reload visions and add to vision list
    public void ReloadVisions()
    {
        foreach (var vision in _proto.EnumeratePrototypes<PleebnarVisionPrototype>())
        {
            _visions.Add(vision);
        }
        _visions.Sort((a, b) => Loc.GetString(a.Name).CompareTo(Loc.GetString(b.Name)));
    }
    //add visions to the radiobutton element
    public void AddVisions()
    {
        VisionsBox.Children.Clear();
        foreach (var entry in _visions)
        {
            AddVision(entry.Name, entry.ID);
        }

    }
    //add a vision (ruddygreat made this one because the radio button is evil, thank you)
    private void AddVision(string name, ProtoId<PleebnarVisionPrototype> vision)
    {
        var currentButtonRef = new Button
        {
            Text = Loc.GetString(name),
            ToolTip = Loc.GetString(_proto.Index(vision).VisionString),
            TextAlign = Label.AlignMode.Right,
            HorizontalAlignment = HAlignment.Center,
            VerticalAlignment = VAlignment.Center,
            SizeFlagsStretchRatio = 1,
            MinSize = new Vector2(340, 20),
            ClipText = true,
        };
        currentButtonRef.OnPressed += _ => OnVisionSelect?.Invoke(vision);
        currentButtonRef.Visible = ButtonIsVisible(currentButtonRef);

        VisionsBox.AddChild(currentButtonRef);
    }
    private bool ButtonIsVisible(Button button)
    {
        return string.IsNullOrEmpty(_searchText) || button.Text == null || button.Text.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || button.ToolTip == null || button.ToolTip.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ;
    }
    //update the state
    public void UpdateState(ProtoId<PleebnarVisionPrototype> vision)
    {
        _vision = vision;//set new vision
    }
    private void UpdateVisibleButtons()
    {
        foreach (var child in VisionsBox.Children)
        {
            if (child is Button button)
                button.Visible = ButtonIsVisible(button);
        }
    }
    private void OnSearchTextChanged(LineEdit.LineEditEventArgs args)
    {
        _searchText = args.Text;

        UpdateVisibleButtons();
        // Reset scroll bar so they can see the relevant results.
        VisionScrollbar.SetScrollValue(Vector2.Zero);
    }
}
