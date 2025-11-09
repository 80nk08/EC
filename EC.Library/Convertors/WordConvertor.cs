using EC.Library.Core;
using System.Text;

namespace EC.Library.Convertors;

public class WordConvertor :  IDisposable
{
    private readonly dynamic _application;
    private readonly TextConvertor _textConvertor;

    public WordConvertor(TextConvertor textConvertor, bool visible)
    {
        _textConvertor = textConvertor;

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

        document.Content.Font.Name = fontName;
        document.Content.Text = _textConvertor.Convert(document.Content.Text.ToString(), encodingType);

        foreach (var footnote in document.Footnotes)
        {
            footnote.Range.Font.Name = fontName;

            footnote.Range.Text = _textConvertor.Convert(footnote.Range.Text.ToString(), encodingType);
        }
        foreach (var sections in document.Sections)
        {
            foreach (var headers in sections.Headers)
            {
                headers.Range.Font.Name = fontName;

                headers.Range.Text = _textConvertor.Convert(headers.Range.Text.ToString(), encodingType);
            }

            foreach (var footer in sections.Footers)
            {
                footer.Range.Font.Name = fontName;

                footer.Range.Text = _textConvertor.Convert(footer.Range.Text.ToString(), encodingType);
            }
        }
        foreach (var endnote in document.Endnotes)
        {
            endnote.Range.Font.Name = fontName;
            endnote.Range.Text = _textConvertor.Convert(endnote.Range.Text.ToString(), encodingType);
        }
        foreach (var shape in document.Shapes)
        {
            if (shape.TextFrame.HasText != 0)
            {
                shape.TextFrame.TextRange.Font.Name = fontName;
                shape.TextFrame.TextRange.Text = _textConvertor.Convert(shape.TextFrame.TextRange.Text.ToString(), encodingType);
            }
        }
        foreach (var comment in document.Comments)
        {
            comment.Range.Font.Name = fontName;
            comment.Range.Text = _textConvertor.Convert(comment.Range.Text.ToString(), encodingType);
        }
        foreach (var table in document.Tables)
        {
            foreach (var cell in table.Range.Cells)
            {
                cell.Range.Font.Name = fontName;
                cell.Range.Text = _textConvertor.Convert(cell.Range.Text.ToString(), encodingType);
            }
        }

        document.Save();
        document.Close();
    }
    
    public void Dispose()
    {
        _application.Quit();
        GC.SuppressFinalize(this);
    }


}
