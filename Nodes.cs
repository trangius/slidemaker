//TODO: most classes seem like copy&paste, make some other fancier solution. How?

public class NCore
{
  public List<NCore> Children { get; } = new List<NCore>();

  public virtual string StartTag => "";
  public virtual string EndTag => "";
  public void AddChild(NCore child)
  {
    Children.Add(child);
  }

  // Recursive rendering to actually create the html-code
  public virtual string RenderHtml()
  {
    string html = StartTag;

    foreach (var child in Children)
    {
      html += child.RenderHtml();
    }

    html += EndTag;
    return html;
  }
}

public class NSlide : NCore
{
  public bool IsReversed { set; get; }
  public override string StartTag => IsReversed
      ? "<div class=\"slide\"><div class=\"slide-content reverse\"><div class=\"text\">\n"
      : "<div class=\"slide\"><div class=\"slide-content\"><div class=\"text\">\n";
  public override string EndTag => "</div></div></div>\n\n";
}

// ********************** TEXT STYLING  ********************** 
public class NH1 : NCore
{
  public override string StartTag => $"<h1>";
  public override string EndTag => "</h1>\n";
}
public class NH2 : NCore
{
  public override string StartTag => $"<h2>";
  public override string EndTag => "</h2>\n";
}
public class NH3 : NCore
{
  public override string StartTag => $"<h3>";
  public override string EndTag => "</h3>\n";
}
public class NItalic : NCore
{
  public override string StartTag => $"<i>";
  public override string EndTag => "</i>\n";
}
public class NCode : NCore
{
  public override string StartTag => $"<span class =\"code\">";
  public override string EndTag => "</span>\n";
}

// TODO: this one should not be able to have children
public class NTxt : NCore
{
  public string? Text { get; set; }
  public override string RenderHtml()
  {
    return Text;
  }
}

// public class NTextContent : NCore
// {
//   public override string StartTag => "<div class=\"text\">\n";
//   public override string EndTag => "</div>\n";
// }
// public class NLRImage : NCore
// {
//   public string? ImagePath { get; set; }
//   public override string StartTag => "<div class=\"image\" style=\"" + CssStyle + "\"><img src=\"" + ImagePath + "\">\n";
//   public override string EndTag => "</div>\n";
// }
// TODO: should there be classes that only have one startTag?
// public class NImage : NCore
// {
//   public string? ImagePath { get; set; }
//   public override string StartTag => "<img src=\"" + ImagePath + "\" style=\"" + CssStyle + "\">\n";
//   public override string EndTag => "";
// }
// public class NParagraph : NCore
// {
//   public override string StartTag => $"<p>{Text}";
//   public override string EndTag => "</p>\n";
// }
//

// ***************************** text styling below
//
