using System.Text.RegularExpressions;
public class Program
{
    // Load input file, convert it, then save it as a new html file
    public static void Main(string[] args)
    {
        string inFile = "input";
        string outFile = "output.html";
        if (args.Length < 2)
        {
            Console.WriteLine("usage: converter <input file> <output file>");
            Console.WriteLine($"using default files input: '{inFile}' and output: '{outFile}'");
        }
        else
        {
            inFile = args[0];
            outFile = args[1];
        }
        if (!File.Exists(inFile))
        {
            Console.WriteLine($"File '{inFile}' not found!");
            return;
        }
        // Do the actual conversion from markup to html:
        string html = "";
        html += File.ReadAllText("_settings/top");
        string markup = File.ReadAllText(inFile);
        html += MlToHtml(markup);
        html += File.ReadAllText("_settings/bottom");
        Console.WriteLine("Saving file...");
        System.IO.File.WriteAllText(outFile, html);
        Console.WriteLine("Done...");
    }

    // Set the regexp to match different classes
    // call the BuildTree, which is a recursive function
    // We then call NCore.RenderHtml() to recursively build the html file
    public static string MlToHtml(string markupText)
    {
        var regex = new Dictionary<Regex, Type>
        {
            // See this to create regex: https://regex101.com/
            { new Regex(@"(?s)\*\*\*((?:(?!\*\*\*).)*)"), typeof(NSlide) },
            { new Regex(@"(?m)^#([^#].*)"),   typeof(NH1) },
            { new Regex(@"(?m)^##([^#].*)"),   typeof(NH2) },
            { new Regex(@"(?m)^###([^#].*)"),   typeof(NH3) },
            { new Regex(@"(?m)^!(.*?)(?:\^(.*))?$"),    typeof(NImage)},
            { new Regex(@"(?m)^([a-öA-Ö].*)\n"),   typeof(NParagraph) },
            // With beginning and end
            { new Regex(@"`(.*?)`"),    typeof(NCode)},
            { new Regex(@"_(.*?)_"),    typeof(NItalic)},
            
            
            // { new Regex(@"^\[(.*?)(?:\^(.*))?$"), typeof(NRLimage) },
            // { new Regex(@"^\](.*?)(?:\^(.*))?$"), typeof(NRLimage) },
        };

        NCore n_root = new NCore(); // just an empty root node to add stuff into.

        BuildTree(n_root, markupText, regex, 0);

        return n_root.RenderHtml();
    }

    // In this method we first find all children on the same level and put them in a list, in order of appearance
    // There can be two kinds of children:
    // tags - these ones represent tags and can have their own children
    // text - these ones are in the very end of the tree and does not have children
    static void BuildTree(NCore parent, string markupText, Dictionary<Regex, Type> regex, int depth)
    {
        //System.Console.WriteLine("==========depth: " + depth);
        depth++;

        int currentIndex = 0;
        int lastIndex = 0;

        // Loop to go find all children on THIS level as a list, before recursing into the children themselves
        // Start iterating through the input string, markupText, from the given starting index (currentIndex)
        while (currentIndex < markupText.Length)
        {
            // These variables will hold the information of the first matching regex pattern
            Match? firstMatch = null;
            Regex? firstPattern = null;
            Type? firstNodeType = null;

            // Iterate through all the regex patterns to find the first matching one
            foreach (var r in regex)
            {
                var match = r.Key.Match(markupText, currentIndex);
                if (match.Success && (firstMatch == null || match.Index < firstMatch.Index))
                {
                    // Update the 'first' variables if this match is earlier in the string than any previous match
                    firstMatch = match;
                    firstPattern = r.Key;
                    firstNodeType = r.Value;
                }
            }

            if (firstMatch != null) // MATCH!
            {
                if (firstMatch.Index > currentIndex)
                {
                    // Add the text _before_ the match as an NTxt
                    var textBefore = markupText.Substring(currentIndex, firstMatch.Index - currentIndex);
                    var txtNode = new NTxt();
                    txtNode.Text = textBefore;
                    parent.AddChild(txtNode);
                }
                if(firstNodeType == typeof(NImage))
                {
                    var imageNode = (NImage)Activator.CreateInstance(firstNodeType);
                    imageNode.ImagePath = firstMatch.Groups[1].Value;
                    imageNode.CssStyle = firstMatch.Groups[2].Value;
                    parent.AddChild(imageNode);
                    currentIndex = firstMatch.Index + firstMatch.Length;
                    lastIndex = currentIndex;
                    continue;
                }

                // Create the match as an instance with C# magic
                NCore child = (NCore)Activator.CreateInstance(firstNodeType);
                parent.AddChild(child);
                currentIndex = firstMatch.Index + firstMatch.Length;
                lastIndex = currentIndex;
                var childMarkup = firstMatch.Groups[1].Value;
                

                BuildTree(child, childMarkup, regex, depth);
            }
            else // NO MATCH
            {
                // add an NTxt
                // TODO: is it possible to smash together with txtNode = new NTxt above?
                var textBefore = markupText.Substring(lastIndex);
                var txtNode = new NTxt();
                txtNode.Text = textBefore;
                parent.AddChild(txtNode);
                break;
            }
        }
        // foreach (var node in parent.Children)
        // {
        //     Console.WriteLine(node.GetType().Name + ":  " + node.RenderHtml());
        // }
    }
}
