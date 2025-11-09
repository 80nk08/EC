using EC.Library.Core;
using System.Text;

namespace EC.Library.Convertors;

public class ExcelConvertor: IDisposable
{
    private readonly dynamic _application;
    private readonly TextConvertor _textConvertor;

    public ExcelConvertor(TextConvertor textConvertor, bool visible)
    {
        _textConvertor = textConvertor;

        var applicationType = Type.GetTypeFromProgID("Excel.Application");
        ArgumentNullException.ThrowIfNull(applicationType);

        var applicationInstance = Activator.CreateInstance(applicationType);
        ArgumentNullException.ThrowIfNull(applicationInstance);

        _application = applicationInstance;
        _application.Visible = visible;
        _application.DisplayAlerts = false;
    }
    public void Convert(string filePath, EncodingType encodingType, string fontName)
    {
        var document = _application.Workbooks.Open(filePath);

        foreach (var sheet in document.Sheets)
        {
            double progressMaximum = sheet.UsedRange.Cells.Count;

            sheet.UsedRange.Font.Name = fontName;

            foreach (var cell in sheet.UsedRange.Cells)
            {

                if (cell.Value == null)  continue;

                cell.Value = _textConvertor.Convert(cell.Value.ToString(), encodingType);
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
