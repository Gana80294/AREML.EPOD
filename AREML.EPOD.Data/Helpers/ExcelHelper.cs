using AREML.EPOD.Core.Dtos.ReverseLogistics;
using AREML.EPOD.Core.Entities.ForwardLogistics;
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

        public ISheet CreateNPOIworksheet(List<P_INV_HEADER_DETAIL> reports, bool isCustomerdetails = true, IWorkbook workbook = null)
        {
            List<string> headings = new List<string>();
            string[] initialHeadings = { "Organization", "Division", "Plant Code","Plant Name", "Invoice No",  "Reference No","Invoice Date", "Invoice Type", "Outbound Delivery","Outbound Delivery Date",
                                       "Customer", "Customer Name",
                                        "Customer Destination","Customer Group Code", "Customer Group Desc", "Sector", "Forward Agent","LR Number","LR Date", "Vehicle No","Carrier", "Vehicle Capacity",
                                       "E-Way Bill No", "E-Way Bill Date","Freight Order",
                                        "Freight Order Date","Proposed Delivery Date", "Vehicle Unloaded Date","Acknowledgement Date", "Acknowledgement Time", "Lead Time","Status","Driver Contact",
                                      "Tracking Link",
                                        "Total Travel Time", "Total Distance",  "Delivery Date","Delivery Time"
                                        };
            for (int i = 0; i < initialHeadings.Length; i++)
            {
                headings.Add(initialHeadings[i]);
            }

            ISheet sheet = workbook.CreateSheet("Invoice details");
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
                    celldef0.SetCellValue(reports[k - 1].ORGANIZATION);
                    ICell celldef1 = rowdef.CreateCell(1);
                    celldef1.SetCellValue(reports[k - 1].DIVISION);
                    ICell celldef2 = rowdef.CreateCell(2);
                    celldef2.SetCellValue(reports[k - 1].PLANT);
                    ICell celldef3 = rowdef.CreateCell(3);
                    celldef3.SetCellValue(reports[k - 1].PLANT_CODE);
                    ICell celldef4 = rowdef.CreateCell(4);
                    celldef4.SetCellValue(reports[k - 1].ODIN);
                    ICell celldef5 = rowdef.CreateCell(5);
                    celldef5.SetCellValue(reports[k - 1].INV_NO);
                    ICell celldef6 = rowdef.CreateCell(6);
                    if (reports[k - 1].INV_DATE.HasValue)
                    {
                        celldef6.SetCellValue(reports[k - 1].INV_DATE.Value.Date);

                    }
                    else
                    {
                        celldef6.SetCellValue("");
                    }
                    ICell celldef7 = rowdef.CreateCell(7);
                    celldef7.SetCellValue(reports[k - 1].INV_TYPE);
                    ICell celldef8 = rowdef.CreateCell(8);
                    celldef8.SetCellValue(reports[k - 1].OUTBOUND_DELIVERY);
                    ICell celldef9 = rowdef.CreateCell(9);
                    if (reports[k - 1].OUTBOUND_DELIVERY_DATE.HasValue)
                    {
                        celldef9.SetCellValue(reports[k - 1].OUTBOUND_DELIVERY_DATE.Value.Date);

                    }
                    else
                    {
                        celldef9.SetCellValue("");
                    }
                    ICell celldef10 = rowdef.CreateCell(10);
                    celldef10.SetCellValue(reports[k - 1].CUSTOMER);
                    ICell celldef11 = rowdef.CreateCell(11);
                    celldef11.SetCellValue(reports[k - 1].CUSTOMER_NAME);
                    ICell celldef12 = rowdef.CreateCell(12);
                    celldef12.SetCellValue(reports[k - 1].CUSTOMER_DESTINATION);
                    ICell celldef13 = rowdef.CreateCell(13);
                    celldef13.SetCellValue(reports[k - 1].CUSTOMER_GROUP);
                    ICell celldef14 = rowdef.CreateCell(14);
                    celldef14.SetCellValue(reports[k - 1].CUSTOMER_GROUP_DESC);
                    ICell celldef15 = rowdef.CreateCell(15);
                    celldef15.SetCellValue(reports[k - 1].SECTOR_DESCRIPTION);
                    ICell celldef16 = rowdef.CreateCell(16);
                    celldef16.SetCellValue(reports[k - 1].FWD_AGENT);
                    ICell celldef17 = rowdef.CreateCell(17);
                    celldef17.SetCellValue(reports[k - 1].LR_NO);
                    ICell celldef18 = rowdef.CreateCell(18);
                    if (reports[k - 1].LR_DATE.HasValue)
                    {
                        celldef18.SetCellValue(reports[k - 1].LR_DATE.Value.Date);

                    }
                    else
                    {
                        celldef18.SetCellValue("");
                    }
                    ICell celldef19 = rowdef.CreateCell(19);
                    celldef19.SetCellValue(reports[k - 1].VEHICLE_NO);
                    ICell celldef20 = rowdef.CreateCell(20);
                    celldef20.SetCellValue(reports[k - 1].CARRIER);
                    ICell celldef21 = rowdef.CreateCell(21);
                    celldef21.SetCellValue(reports[k - 1].VEHICLE_CAPACITY);
                    ICell celldef22 = rowdef.CreateCell(22);
                    celldef22.SetCellValue(reports[k - 1].EWAYBILL_NO);
                    ICell celldef23 = rowdef.CreateCell(23);
                    if (reports[k - 1].EWAYBILL_DATE.HasValue)
                    {
                        celldef23.SetCellValue(reports[k - 1].EWAYBILL_DATE.Value.Date);

                    }
                    else
                    {
                        celldef23.SetCellValue("");
                    }
                    ICell celldef24 = rowdef.CreateCell(24);
                    celldef24.SetCellValue(reports[k - 1].FREIGHT_ORDER);
                    ICell celldef25 = rowdef.CreateCell(25);
                    if (reports[k - 1].FREIGHT_ORDER_DATE.HasValue)
                    {
                        celldef25.SetCellValue(reports[k - 1].FREIGHT_ORDER_DATE.Value.Date);

                    }
                    else
                    {
                        celldef25.SetCellValue("");
                    }
                    ICell celldef26 = rowdef.CreateCell(26);
                    if (reports[k - 1].PROPOSED_DELIVERY_DATE.HasValue)
                    {
                        celldef26.SetCellValue(reports[k - 1].PROPOSED_DELIVERY_DATE.Value.Date);

                    }
                    else
                    {
                        celldef26.SetCellValue("");
                    }
                    ICell celldef27 = rowdef.CreateCell(27);
                    if (reports[k - 1].VEHICLE_REPORTED_DATE.HasValue)
                    {
                        celldef27.SetCellValue(reports[k - 1].VEHICLE_REPORTED_DATE.Value.Date);

                    }
                    else
                    {
                        celldef27.SetCellValue("");
                    }
                    ICell celldef28 = rowdef.CreateCell(28);
                    if (reports[k - 1].ACTUAL_DELIVERY_DATE.HasValue)
                    {
                        celldef28.SetCellValue(reports[k - 1].ACTUAL_DELIVERY_DATE.Value.Date.ToString("dd/MM/yyyy"));

                    }
                    else
                    {
                        celldef28.SetCellValue("");
                    }
                    ICell celldef29 = rowdef.CreateCell(29);
                    if (reports[k - 1].ACTUAL_DELIVERY_DATE.HasValue)
                    {
                        celldef29.SetCellValue(reports[k - 1].ACTUAL_DELIVERY_DATE.Value.ToString("HH:mm:ss"));

                    }
                    else
                    {
                        celldef29.SetCellValue("");
                    }
                    ICell celldef30 = rowdef.CreateCell(30);
                    celldef30.SetCellValue(reports[k - 1].TRANSIT_LEAD_TIME);
                    ICell celldef31 = rowdef.CreateCell(31);
                    celldef31.SetCellValue(reports[k - 1].STATUS);
                    ICell celldef32 = rowdef.CreateCell(32);
                    celldef32.SetCellValue(reports[k - 1].DRIVER_CONTACT);
                    ICell celldef33 = rowdef.CreateCell(33);
                    celldef33.SetCellValue(reports[k - 1].TRACKING_LINK);
                    ICell celldef34 = rowdef.CreateCell(34);
                    celldef34.SetCellValue(reports[k - 1].TOTAL_TRAVEL_TIME.ToString());
                    ICell celldef35 = rowdef.CreateCell(35);
                    celldef35.SetCellValue(reports[k - 1].TOTAL_DISTANCE.ToString());
                    ICell celldef36 = rowdef.CreateCell(36);
                    celldef36.SetCellValue(reports[k - 1].DELIVERY_DATE);
                    ICell celldef37 = rowdef.CreateCell(37);
                    celldef37.SetCellValue(reports[k - 1].DELIVERY_TIME);
                }

                return sheet;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ISheet CreateActionHistoryWorksheet(List<UserActionHistoryView> reports, bool isCustomerdetails = true, IWorkbook workbook = null)
        {
            List<string> headings = new List<string>();
            string[] initialHeadings = { "Username", "Location", "Action", "Invoice No", "ChangesDetected", "Date", "Time" };
            for (int i = 0; i < initialHeadings.Length; i++)
            {
                headings.Add(initialHeadings[i]);
            }

            ISheet sheet = workbook.CreateSheet("Action log");
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
                    celldef0.SetCellValue(reports[k - 1].UserName);
                    ICell celldef1 = rowdef.CreateCell(1);
                    celldef1.SetCellValue(reports[k - 1].Location);
                    ICell celldef2 = rowdef.CreateCell(2);
                    celldef2.SetCellValue(reports[k - 1].Action);
                    ICell celldef3 = rowdef.CreateCell(3);
                    celldef3.SetCellValue(reports[k - 1].InvoiceNumber);
                    ICell celldef4 = rowdef.CreateCell(4);
                    celldef4.SetCellValue(reports[k - 1].ChangesDetected);
                    ICell celldef5 = rowdef.CreateCell(5);
                    celldef5.SetCellValue(reports[k - 1].DateTime.Date.ToString("dd/MM/yyyy"));
                    ICell celldef6 = rowdef.CreateCell(6);
                    celldef6.SetCellValue(reports[k - 1].DateTime.ToString("HH:mm:ss"));
                }
                return sheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ISheet CreateNPOIWorksheet(List<ReversePodReportDto> reports, IWorkbook workbook = null)
        {
            List<string> headings = new List<string>();
            //string[] initialHeadings = { "DC Number", "DC Date", "Claim Type", "Customer", "Customer Name", "Plant", "Plant Name", "Material Code","Quantity","Handovered Quantity","Customer Pending Quantity," +
            //        "Received Quantity","DC Pending Quantity","Status","SLA Date","Pending Days","Remarks","Invoice Total Quantity","Invoice Billed Quantity","Invoice Balance Quantity"};
            //for (int i = 0; i < initialHeadings.Length; i++)
            //{
            //    headings.Add(initialHeadings[i]);
            //}

            headings.Add("DC Number");
            headings.Add("DC Date");
            headings.Add("Claim Type");
            headings.Add("Customer");
            headings.Add("Customer Name");
            headings.Add("Plant");
            headings.Add("Plant Name");
            headings.Add("Material Code");
            headings.Add("Quantity");
            headings.Add("Handovered Quantity");
            headings.Add("Customer Pending Quantity");
            headings.Add("Receieved Quantity");
            headings.Add("Dc Pending Quantity");
            headings.Add("Status");
            headings.Add("SLA Date");
            headings.Add("Pending Days");
            headings.Add("Remarks");
            headings.Add("Invoice Total Quantity");
            headings.Add("Invoice Billed Quantity");
            headings.Add("Invoice Balance Quantity");


            ISheet sheet = workbook.CreateSheet("Rpod details");
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
                    celldef0.SetCellValue(reports[k - 1].DC_NUMBER);
                    ICell celldef1 = rowdef.CreateCell(1);
                    if (reports[k - 1].DC_DATE.HasValue)
                    {
                        celldef1.SetCellValue(reports[k - 1].DC_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef1.SetCellValue("");
                    }
                    ICell celldef2 = rowdef.CreateCell(2);
                    celldef2.SetCellValue(reports[k - 1].CLAIM_TYPE);
                    ICell celldef3 = rowdef.CreateCell(3);
                    celldef3.SetCellValue(reports[k - 1].CUSTOMER);
                    ICell celldef4 = rowdef.CreateCell(4);
                    celldef4.SetCellValue(reports[k - 1].CUSTOMER_NAME);
                    ICell celldef5 = rowdef.CreateCell(5);
                    celldef5.SetCellValue(reports[k - 1].PLANT);
                    ICell celldef6 = rowdef.CreateCell(6);
                    celldef6.SetCellValue(reports[k - 1].PLANT_NAME);
                    ICell celldef7 = rowdef.CreateCell(7);
                    celldef7.SetCellValue(reports[k - 1].MATERIAL_CODE);
                    ICell celldef8 = rowdef.CreateCell(8);
                    celldef8.SetCellValue(reports[k - 1].QUANTITY.ToString());
                    ICell celldef9 = rowdef.CreateCell(9);
                    celldef9.SetCellValue(reports[k - 1].HAND_OVERED_QUANTITY.ToString());
                    ICell celldef10 = rowdef.CreateCell(10);
                    celldef10.SetCellValue(reports[k - 1].CUSTOMER_PENDING_QUANTITY.ToString());
                    ICell celldef11 = rowdef.CreateCell(11);
                    celldef11.SetCellValue(reports[k - 1].RECEIVED_QUANTITY.ToString());
                    ICell celldef12 = rowdef.CreateCell(12);
                    celldef12.SetCellValue(reports[k - 1].DC_PENDING_QUANTITY.ToString());

                    ICell celldef13 = rowdef.CreateCell(13);
                    celldef13.SetCellValue(reports[k - 1].STATUS);

                    ICell celldef14 = rowdef.CreateCell(14);
                    if (reports[k - 1].SLA_DATE.HasValue)
                    {
                        celldef14.SetCellValue(reports[k - 1].SLA_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef14.SetCellValue("");
                    }
                    ICell celldef15 = rowdef.CreateCell(15);
                    celldef15.SetCellValue(reports[k - 1].PENDING_DAYS);
                    ICell celldef16 = rowdef.CreateCell(16);
                    celldef16.SetCellValue(reports[k - 1].REMARKS);
                    ICell celldef17 = rowdef.CreateCell(17);
                    celldef17.SetCellValue(reports[k - 1].INVOICE_TOTAL_QTY.ToString());
                    ICell celldef18 = rowdef.CreateCell(18);
                    celldef18.SetCellValue(reports[k - 1].INVOICE_BILLED_QTY.ToString());
                    ICell celldef19 = rowdef.CreateCell(19);
                    celldef19.SetCellValue(reports[k - 1].INVOICE_BALANCE_QTY.ToString());
                }
                return sheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ISheet CreateNPOIworksheetReport(List<ReportInvoice> reports, IWorkbook workbook = null)
        {
            List<string> headings = new List<string>();
            string[] initialHeadings = { "Organization", "Division", "Plant Code","Plant Name", "Invoice No",  "Reference No","Invoice Date", "Invoice Type",
                                         "Outbound Delivery","Outbound Delivery Date", "Customer", "Customer Name",
                                        "Customer Destination","Distance","Customer Group Code", "Customer Group Desc", "Sector", "Forward Agent","LR Number","LR Date",
                                        "Vehicle No","Carrier", "Vehicle Capacity", "E-Way Bill No", "E-Way Bill Date","Freight Order",
                                        "Freight Order Date","Proposed Delivery Date", "Vehicle Unloaded Date","Acknowledgement Date", "Acknowledgement Time",
                                         "Lead Time","Material Code","Material Description","UOM","Quantity","Gross Weight",
                                         "Received Quantity","Customer Remarks","Status","Driver Contact","Tracking Link",
                                        "Total Travel Time", "Total Distance",  "Delivery Date","Delivery Time"
                                        };
            for (int i = 0; i < initialHeadings.Length; i++)
            {
                headings.Add(initialHeadings[i]);
            }
            ISheet sheet = workbook.CreateSheet("Invoice details");
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
                    celldef0.SetCellValue(reports[k - 1].ORGANIZATION);
                    ICell celldef1 = rowdef.CreateCell(1);
                    celldef1.SetCellValue(reports[k - 1].DIVISION);
                    ICell celldef2 = rowdef.CreateCell(2);
                    celldef2.SetCellValue(reports[k - 1].PLANT);
                    ICell celldef3 = rowdef.CreateCell(3);
                    celldef3.SetCellValue(reports[k - 1].PLANT_NAME);
                    ICell celldef4 = rowdef.CreateCell(4);
                    celldef4.SetCellValue(reports[k - 1].ODIN);
                    ICell celldef5 = rowdef.CreateCell(5);
                    celldef5.SetCellValue(reports[k - 1].INV_NO);
                    ICell celldef6 = rowdef.CreateCell(6);
                    if (reports[k - 1].INV_DATE.HasValue)
                    {
                        celldef6.SetCellValue(reports[k - 1].INV_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef6.SetCellValue("");
                    }
                    ICell celldef7 = rowdef.CreateCell(7);
                    celldef7.SetCellValue(reports[k - 1].INV_TYPE);
                    ICell celldef8 = rowdef.CreateCell(8);
                    celldef8.SetCellValue(reports[k - 1].OUTBOUND_DELIVERY);
                    ICell celldef9 = rowdef.CreateCell(9);
                    if (reports[k - 1].OUTBOUND_DELIVERY_DATE.HasValue)
                    {
                        celldef9.SetCellValue(reports[k - 1].OUTBOUND_DELIVERY_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef9.SetCellValue("");
                    }
                    ICell celldef10 = rowdef.CreateCell(10);
                    celldef10.SetCellValue(reports[k - 1].CUSTOMER);
                    ICell celldef11 = rowdef.CreateCell(11);
                    celldef11.SetCellValue(reports[k - 1].CUSTOMER_NAME);
                    ICell celldef12 = rowdef.CreateCell(12);
                    celldef12.SetCellValue(reports[k - 1].CUSTOMER_DESTINATION);
                    ICell celldef13 = rowdef.CreateCell(13);
                    celldef13.SetCellValue(reports[k - 1].DISTANCE);
                    ICell celldef14 = rowdef.CreateCell(14);
                    celldef14.SetCellValue(reports[k - 1].CUSTOMER_GROUP);
                    ICell celldef15 = rowdef.CreateCell(15);
                    celldef15.SetCellValue(reports[k - 1].CUSTOMER_GROUP_DESC);
                    ICell celldef16 = rowdef.CreateCell(16);
                    celldef16.SetCellValue(reports[k - 1].SECTOR_DESCRIPTION);
                    ICell celldef17 = rowdef.CreateCell(17);
                    celldef17.SetCellValue(reports[k - 1].FWD_AGENT);
                    ICell celldef18 = rowdef.CreateCell(18);
                    celldef18.SetCellValue(reports[k - 1].LR_NO);
                    ICell celldef19 = rowdef.CreateCell(19);
                    if (reports[k - 1].LR_DATE.HasValue)
                    {
                        celldef19.SetCellValue(reports[k - 1].LR_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef19.SetCellValue("");
                    }
                    ICell celldef20 = rowdef.CreateCell(20);
                    celldef20.SetCellValue(reports[k - 1].VEHICLE_NO);
                    ICell celldef21 = rowdef.CreateCell(21);
                    celldef21.SetCellValue(reports[k - 1].CARRIER);
                    ICell celldef22 = rowdef.CreateCell(22);
                    celldef22.SetCellValue(reports[k - 1].VEHICLE_CAPACITY);
                    ICell celldef23 = rowdef.CreateCell(23);
                    celldef23.SetCellValue(reports[k - 1].EWAYBILL_NO);
                    ICell celldef24 = rowdef.CreateCell(24);
                    if (reports[k - 1].EWAYBILL_DATE.HasValue)
                    {
                        celldef24.SetCellValue(reports[k - 1].EWAYBILL_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef24.SetCellValue("");
                    }
                    ICell celldef25 = rowdef.CreateCell(25);
                    celldef25.SetCellValue(reports[k - 1].FREIGHT_ORDER);
                    ICell celldef26 = rowdef.CreateCell(26);
                    if (reports[k - 1].FREIGHT_ORDER_DATE.HasValue)
                    {
                        celldef26.SetCellValue(reports[k - 1].FREIGHT_ORDER_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef26.SetCellValue("");
                    }
                    ICell celldef27 = rowdef.CreateCell(27);
                    if (reports[k - 1].PROPOSED_DELIVERY_DATE.HasValue)
                    {
                        celldef27.SetCellValue(reports[k - 1].PROPOSED_DELIVERY_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef27.SetCellValue("");
                    }
                    ICell celldef28 = rowdef.CreateCell(28);
                    if (reports[k - 1].VEHICLE_REPORTED_DATE.HasValue)
                    {
                        celldef28.SetCellValue(reports[k - 1].VEHICLE_REPORTED_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef28.SetCellValue("");
                    }
                    ICell celldef29 = rowdef.CreateCell(29);
                    if (reports[k - 1].ACTUAL_DELIVERY_DATE.HasValue)
                    {
                        celldef29.SetCellValue(reports[k - 1].ACTUAL_DELIVERY_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef29.SetCellValue("");
                    }
                    ICell celldef30 = rowdef.CreateCell(30);
                    if (reports[k - 1].ACTUAL_DELIVERY_DATE.HasValue)
                    {
                        celldef30.SetCellValue(reports[k - 1].ACTUAL_DELIVERY_DATE.Value.ToString("HH:mm:ss"));
                    }
                    else
                    {
                        celldef30.SetCellValue("");
                    }
                    ICell celldef31 = rowdef.CreateCell(31);
                    celldef31.SetCellValue(reports[k - 1].TRANSIT_LEAD_TIME);
                    ICell celldef32 = rowdef.CreateCell(32);
                    celldef32.SetCellValue(reports[k - 1].MATERIAL_CODE);
                    ICell celldef33 = rowdef.CreateCell(33);
                    celldef33.SetCellValue(reports[k - 1].MATERIAL_DESCRIPTION);
                    ICell celldef34 = rowdef.CreateCell(34);
                    celldef34.SetCellValue(reports[k - 1].QUANTITY_UOM);
                    ICell celldef35 = rowdef.CreateCell(35);
                    celldef35.SetCellValue(reports[k - 1].QUANTITY.HasValue ? reports[k - 1].QUANTITY.ToString() : "");
                    ICell celldef36 = rowdef.CreateCell(36);
                    celldef36.SetCellValue(reports[k - 1].ITEM_WEIGHT);
                    ICell celldef37 = rowdef.CreateCell(37);
                    celldef37.SetCellValue(reports[k - 1].RECEIVED_QUANTITY.HasValue ? reports[k - 1].RECEIVED_QUANTITY.ToString() : "");
                    ICell celldef38 = rowdef.CreateCell(38);
                    celldef38.SetCellValue(reports[k - 1].STATUS);
                    ICell celldef39 = rowdef.CreateCell(39);
                    celldef39.SetCellValue(reports[k - 1].REMARKS);
                    ICell celldef40 = rowdef.CreateCell(40);
                    celldef40.SetCellValue(reports[k - 1].DRIVER_CONTACT);
                    ICell celldef41 = rowdef.CreateCell(41);
                    celldef41.SetCellValue(reports[k - 1].TRACKING_LINK);
                    ICell celldef42 = rowdef.CreateCell(42);
                    celldef42.SetCellValue(reports[k - 1].TOTAL_TRAVEL_TIME.ToString());
                    ICell celldef43 = rowdef.CreateCell(43);
                    celldef43.SetCellValue(reports[k - 1].TOTAL_DISTANCE.ToString());
                    ICell celldef44 = rowdef.CreateCell(44);
                    celldef44.SetCellValue(reports[k - 1].DELIVERY_DATE);
                    ICell celldef45 = rowdef.CreateCell(45);
                    celldef45.SetCellValue(reports[k - 1].DELIVERY_TIME);
                }
                return sheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ISheet CreateNPOIworksheetForAutomation(List<ReportInvoice> reports, IWorkbook workbook = null)
        {
            List<string> headings = new List<string>();
            string[] initialHeadings = { "Organization", "Division", "Plant Code","Plant Name", "Invoice No",  "Reference No","Invoice Date", "Invoice Type",
                                         "Outbound Delivery","Outbound Delivery Date", "Customer", "Customer Name",
                                        "Customer Destination","Distance","Customer Group Code", "Customer Group Desc", "Sector", "Forward Agent","LR Number","LR Date",
                                        "Vehicle No","Carrier", "Vehicle Capacity", "E-Way Bill No", "E-Way Bill Date","Freight Order",
                                        "Freight Order Date","Proposed Delivery Date", "Vehicle Unloaded Date","Acknowledgement Date", "Acknowledgement Time",
                                         "Lead Time","Material Code","Material Description","UOM","Quantity","Gross Weight",
                                         "Received Quantity","Customer Remarks","Status"
                                        };
            for (int i = 0; i < initialHeadings.Length; i++)
            {
                headings.Add(initialHeadings[i]);
            }

            ISheet sheet = workbook.CreateSheet("Invoice details");
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
                    celldef0.SetCellValue(reports[k - 1].ORGANIZATION);
                    ICell celldef1 = rowdef.CreateCell(1);
                    celldef1.SetCellValue(reports[k - 1].DIVISION);
                    ICell celldef2 = rowdef.CreateCell(2);
                    celldef2.SetCellValue(reports[k - 1].PLANT);
                    ICell celldef3 = rowdef.CreateCell(3);
                    celldef3.SetCellValue(reports[k - 1].PLANT_NAME);
                    ICell celldef4 = rowdef.CreateCell(4);
                    celldef4.SetCellValue(reports[k - 1].ODIN);
                    ICell celldef5 = rowdef.CreateCell(5);
                    celldef5.SetCellValue(reports[k - 1].INV_NO);
                    ICell celldef6 = rowdef.CreateCell(6);
                    if (reports[k - 1].INV_DATE.HasValue)
                    {
                        celldef6.SetCellValue(reports[k - 1].INV_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef6.SetCellValue("");
                    }
                    ICell celldef7 = rowdef.CreateCell(7);
                    celldef7.SetCellValue(reports[k - 1].INV_TYPE);
                    ICell celldef8 = rowdef.CreateCell(8);
                    celldef8.SetCellValue(reports[k - 1].OUTBOUND_DELIVERY);
                    ICell celldef9 = rowdef.CreateCell(9);
                    if (reports[k - 1].OUTBOUND_DELIVERY_DATE.HasValue)
                    {
                        celldef9.SetCellValue(reports[k - 1].OUTBOUND_DELIVERY_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef9.SetCellValue("");
                    }
                    ICell celldef10 = rowdef.CreateCell(10);
                    celldef10.SetCellValue(reports[k - 1].CUSTOMER);
                    ICell celldef11 = rowdef.CreateCell(11);
                    celldef11.SetCellValue(reports[k - 1].CUSTOMER_NAME);
                    ICell celldef12 = rowdef.CreateCell(12);
                    celldef12.SetCellValue(reports[k - 1].CUSTOMER_DESTINATION);
                    ICell celldef13 = rowdef.CreateCell(13);
                    celldef13.SetCellValue(reports[k - 1].DISTANCE);
                    ICell celldef14 = rowdef.CreateCell(14);
                    celldef14.SetCellValue(reports[k - 1].CUSTOMER_GROUP);
                    ICell celldef15 = rowdef.CreateCell(15);
                    celldef15.SetCellValue(reports[k - 1].CUSTOMER_GROUP_DESC);
                    ICell celldef16 = rowdef.CreateCell(16);
                    celldef16.SetCellValue(reports[k - 1].SECTOR_DESCRIPTION);
                    ICell celldef17 = rowdef.CreateCell(17);
                    celldef17.SetCellValue(reports[k - 1].FWD_AGENT);
                    ICell celldef18 = rowdef.CreateCell(18);
                    celldef18.SetCellValue(reports[k - 1].LR_NO);
                    ICell celldef19 = rowdef.CreateCell(19);
                    if (reports[k - 1].LR_DATE.HasValue)
                    {
                        celldef19.SetCellValue(reports[k - 1].LR_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef19.SetCellValue("");
                    }
                    ICell celldef20 = rowdef.CreateCell(20);
                    celldef20.SetCellValue(reports[k - 1].VEHICLE_NO);
                    ICell celldef21 = rowdef.CreateCell(21);
                    celldef21.SetCellValue(reports[k - 1].CARRIER);
                    ICell celldef22 = rowdef.CreateCell(22);
                    celldef22.SetCellValue(reports[k - 1].VEHICLE_CAPACITY);
                    ICell celldef23 = rowdef.CreateCell(23);
                    celldef23.SetCellValue(reports[k - 1].EWAYBILL_NO);
                    ICell celldef24 = rowdef.CreateCell(24);
                    if (reports[k - 1].EWAYBILL_DATE.HasValue)
                    {
                        celldef24.SetCellValue(reports[k - 1].EWAYBILL_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef24.SetCellValue("");
                    }
                    ICell celldef25 = rowdef.CreateCell(25);
                    celldef25.SetCellValue(reports[k - 1].FREIGHT_ORDER);
                    ICell celldef26 = rowdef.CreateCell(26);
                    if (reports[k - 1].FREIGHT_ORDER_DATE.HasValue)
                    {
                        celldef26.SetCellValue(reports[k - 1].FREIGHT_ORDER_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef26.SetCellValue("");
                    }
                    ICell celldef27 = rowdef.CreateCell(27);
                    if (reports[k - 1].PROPOSED_DELIVERY_DATE.HasValue)
                    {
                        celldef27.SetCellValue(reports[k - 1].PROPOSED_DELIVERY_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef27.SetCellValue("");
                    }
                    ICell celldef28 = rowdef.CreateCell(28);
                    if (reports[k - 1].VEHICLE_REPORTED_DATE.HasValue)
                    {
                        celldef28.SetCellValue(reports[k - 1].VEHICLE_REPORTED_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef28.SetCellValue("");
                    }
                    ICell celldef29 = rowdef.CreateCell(29);
                    if (reports[k - 1].ACTUAL_DELIVERY_DATE.HasValue)
                    {
                        celldef29.SetCellValue(reports[k - 1].ACTUAL_DELIVERY_DATE.Value.Date.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        celldef29.SetCellValue("");
                    }
                    ICell celldef30 = rowdef.CreateCell(30);
                    if (reports[k - 1].ACTUAL_DELIVERY_DATE.HasValue)
                    {
                        celldef30.SetCellValue(reports[k - 1].ACTUAL_DELIVERY_DATE.Value.ToString("HH:mm:ss"));
                    }
                    else
                    {
                        celldef30.SetCellValue("");
                    }
                    ICell celldef31 = rowdef.CreateCell(31);
                    celldef31.SetCellValue(reports[k - 1].TRANSIT_LEAD_TIME);
                    ICell celldef32 = rowdef.CreateCell(32);
                    celldef32.SetCellValue(reports[k - 1].MATERIAL_CODE);
                    ICell celldef33 = rowdef.CreateCell(33);
                    celldef33.SetCellValue(reports[k - 1].MATERIAL_DESCRIPTION);
                    ICell celldef34 = rowdef.CreateCell(34);
                    celldef34.SetCellValue(reports[k - 1].QUANTITY_UOM);
                    ICell celldef35 = rowdef.CreateCell(35);
                    celldef35.SetCellValue(reports[k - 1].QUANTITY.HasValue ? reports[k - 1].QUANTITY.ToString() : "");
                    ICell celldef36 = rowdef.CreateCell(36);
                    celldef36.SetCellValue(reports[k - 1].ITEM_WEIGHT);
                    ICell celldef37 = rowdef.CreateCell(37);
                    celldef37.SetCellValue(reports[k - 1].RECEIVED_QUANTITY.HasValue ? reports[k - 1].RECEIVED_QUANTITY.ToString() : "");
                    ICell celldef38 = rowdef.CreateCell(38);
                    celldef38.SetCellValue(reports[k - 1].STATUS);
                    ICell celldef39 = rowdef.CreateCell(39);
                    celldef39.SetCellValue(reports[k - 1].REMARKS);
                }
                return sheet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
