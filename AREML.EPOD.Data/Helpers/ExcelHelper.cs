using AREML.EPOD.Core.Entities.Master;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Helpers
{
    public class ExcelHelper
    {
        public ExcelHelper()
        {

        }
        public ISheet CreateNPOIExportUserworksheet(List<ExportUserModel> reports, IWorkbook workbook = null)
        {
            string rol = reports[0].RoleName;
            List<string> headings = new List<string>();
            string[] initialHeadings = { "UserCode", "UserName", "Email ID", "Mobile", "RoleName" };
            for (int i = 0; i < initialHeadings.Length; i++)
            {
                headings.Add(initialHeadings[i]);
            }
         
            if (rol == "Amararaja User")
            {
                string[] additionalHeadings = { "Organization", "Plant", "Sales Groups" };
                for (int i = 0; i < additionalHeadings.Length; i++)
                {
                    headings.Add(additionalHeadings[i]);
                }

            }
            ISheet sheet = workbook.CreateSheet("Users");
            try
            {
                IRow row = sheet.CreateRow(0);
                var font = workbook.CreateFont();
                font.FontHeightInPoints = 11;
                font.FontName = "Calibri";
                font.IsBold = true;
                var titleStyle = workbook.CreateCellStyle();
                titleStyle.SetFont(font);

                for (var k = 0; k < headings.Count; k++)
                {
                    ICell cell = row.CreateCell(k);
                    cell.CellStyle = titleStyle;
                    cell.SetCellValue(headings[k]);
                }

                for (var k = 1; k <= reports.Count; k++)
                {
                    IRow rowdef = sheet.CreateRow(k);
                    ICell celldef0 = rowdef.CreateCell(0);
                    celldef0.SetCellValue(reports[k - 1].UserCode);
                    ICell celldef1 = rowdef.CreateCell(1);
                    celldef1.SetCellValue(reports[k - 1].UserName);
                    ICell celldef2 = rowdef.CreateCell(2);
                    celldef2.SetCellValue(reports[k - 1].EmailID);
                    ICell celldef3 = rowdef.CreateCell(3);
                    celldef3.SetCellValue(reports[k - 1].Mobile);
                    ICell celldef4 = rowdef.CreateCell(4);
                    celldef4.SetCellValue(reports[k - 1].RoleName);
                    if (reports[k - 1].isAmuser)
                    {
                        ICell celldef5 = rowdef.CreateCell(5);
                        celldef5.SetCellValue(reports[k - 1].Organization);
                        ICell celldef6 = rowdef.CreateCell(6);
                        celldef6.SetCellValue(reports[k - 1].Plant);
                        ICell celldef7 = rowdef.CreateCell(7);
                        celldef7.SetCellValue(reports[k - 1].SalesGroups);
                    }

                }

                return sheet;

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
