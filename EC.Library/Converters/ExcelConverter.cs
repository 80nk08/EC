using EC.Library.Core;
using System.Text;

namespace EC.Library.Converters;

public class ExcelConverter: IDisposable
{
    private readonly dynamic _application;
    private readonly TextConverter _textConverter;

    public ExcelConverter(TextConverter textConverter, bool visible)
    {
        _textConverter = textConverter;

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

        try
        {
            ArgumentNullException.ThrowIfNull(document);

            foreach (var sheet in document.Sheets)
            {
                double progressMaximum = sheet.UsedRange.Cells.Count;

                sheet.UsedRange.Font.Name = fontName;

                foreach (var cell in sheet.UsedRange.Cells)
                {

                    if (cell.Value == null) continue;

                    cell.Value = _textConverter.Convert(cell.Value.ToString(), encodingType);
                }
            }

            document.Save();
        }
        finally
        {
            document.Close();
            ReleaseComObject(document);
        }

    }
    
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