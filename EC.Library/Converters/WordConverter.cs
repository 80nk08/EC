using EC.Library.Core;
using System.Text;

namespace EC.Library.Converters;

public class WordConverter :  IDisposable
{
    private readonly dynamic _application;
    private readonly TextConverter _textConverter;

    public WordConverter(TextConverter textConverter, bool visible)
    {
        _textConverter = textConverter;

        var applicationType = Type.GetTypeFromProgID("Word.Application");
        ArgumentNullException.ThrowIfNull(applicationType);

        var applicationInstance = Activator.CreateInstance(applicationType);
        ArgumentNullException.ThrowIfNull(applicationInstance);

        _application = applicationInstance;
        _application.Visible = visible;
        _application.DisplayAlerts = false;
    }
    public void Convert(string filePath, EncodingType encodingType, string fontName)
    {
        var document = _application.Documents.Open(filePath);
        try
        {
            document.Content.Font.Name = fontName;
            document.Content.Text = _textConverter.Convert(document.Content.Text.ToString(), encodingType);

            foreach (var footnote in document.Footnotes)
            {
                footnote.Range.Font.Name = fontName;

                footnote.Range.Text = _textConverter.Convert(footnote.Range.Text.ToString(), encodingType);
            }

            foreach (var sections in document.Sections)
            {
                foreach (var headers in sections.Headers)
                {
                    headers.Range.Font.Name = fontName;

                    headers.Range.Text = _textConverter.Convert(headers.Range.Text.ToString(), encodingType);
                }

                foreach (var footer in sections.Footers)
                {
                    footer.Range.Font.Name = fontName;

                    footer.Range.Text = _textConverter.Convert(footer.Range.Text.ToString(), encodingType);
                }
            }

            foreach (var endnote in document.Endnotes)
            {
                endnote.Range.Font.Name = fontName;
                endnote.Range.Text = _textConverter.Convert(endnote.Range.Text.ToString(), encodingType);
            }

            foreach (var shape in document.Shapes)
            {
                if (shape.TextFrame.HasText != 0)
                {
                    shape.TextFrame.TextRange.Font.Name = fontName;
                    shape.TextFrame.TextRange.Text =
                        _textConverter.Convert(shape.TextFrame.TextRange.Text.ToString(), encodingType);
                }
            }

            foreach (var comment in document.Comments)
            {
                comment.Range.Font.Name = fontName;
                comment.Range.Text = _textConverter.Convert(comment.Range.Text.ToString(), encodingType);
            }

            foreach (var table in document.Tables)
            {
                foreach (var cell in table.Range.Cells)
                {
                    cell.Range.Font.Name = fontName;
                    cell.Range.Text = _textConverter.Convert(cell.Range.Text.ToString(), encodingType);
                }
            }

            document.Save();
        }
        finally
        {
            if (document != null)
            {
                document.Close();
                ReleaseComObject(document);
            }
        }
    }
    
    // You should call Marshal.ReleaseComObject (or otherwise correctly release it) to avoid memory leaks.
    private static void ReleaseComObject(object? obj)
    {
        if (obj != null)
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
        }
    }
    
    public void Dispose()
    {
        _application.Quit();
        GC.SuppressFinalize(this);
    }


}
