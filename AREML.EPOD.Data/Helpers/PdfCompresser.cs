using AREML.EPOD.Core.Entities.ForwardLogistics;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Utils;
using iText.Layout;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Drawing.Imaging;
using System.Drawing;

namespace AREML.EPOD.Data.Helpers
{
    public class PdfCompresser
    {
        #region itext7
        public ConvertedAttachmentProps ConvertImagetoPDF(string filename, byte[] filebytes)
        {
            ConvertedAttachmentProps convertedAttachmentProps = new ConvertedAttachmentProps();
            var spltfn = filename.Split('.');
            var ext = spltfn[spltfn.Length - 1];
            if (ext.ToLower() == "pdf")
            {
                convertedAttachmentProps.Filename = filename;
                convertedAttachmentProps.PDFcontent = filebytes;

                return convertedAttachmentProps;
            }
            else
            {
                var fileSize = filebytes.Length / (1024 * 1024);
                if (fileSize < 1)
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var pdfWriter = new iText.Kernel.Pdf.PdfWriter(ms))
                        using (var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfWriter))
                        {
                            using (var document = new iText.Layout.Document(pdfDocument))
                            {
                                var imageData = iText.IO.Image.ImageDataFactory.Create(filebytes);
                                var image = new iText.Layout.Element.Image(imageData);
                                pdfDocument.SetDefaultPageSize(new iText.Kernel.Geom.PageSize(image.GetImageWidth(), image.GetImageHeight()));
                                document.Add(image);
                            }
                        }
                        convertedAttachmentProps.PDFcontent = ms.ToArray();
                        convertedAttachmentProps.Filename = ConvertFilenametoPDF(filename);
                    }

                    return convertedAttachmentProps;
                }
                else
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var pdfWriter = new iText.Kernel.Pdf.PdfWriter(ms))
                        using (var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfWriter))
                        {
                            using (var document = new iText.Layout.Document(pdfDocument))
                            {
                                var imageData = iText.IO.Image.ImageDataFactory.Create(CompressImage(filebytes, 15));
                                var image = new iText.Layout.Element.Image(imageData);
                                pdfDocument.SetDefaultPageSize(new iText.Kernel.Geom.PageSize(image.GetImageWidth(), image.GetImageHeight()));
                                image.SetFixedPosition(0, 0);
                                image.ScaleAbsolute(pdfDocument.GetDefaultPageSize().GetWidth(), pdfDocument.GetDefaultPageSize().GetHeight());
                                document.Add(image);
                            }
                        }
                        convertedAttachmentProps.PDFcontent = ms.ToArray();
                        convertedAttachmentProps.Filename = ConvertFilenametoPDF(filename);
                    }

                    return convertedAttachmentProps;
                }
            }
        }

        public string ConvertFilenametoPDF(string name)
        {
            return System.IO.Path.ChangeExtension(name, ".pdf");
        }

        private static byte[] CompressImage(byte[] filebytes, int newQuality)
        {
            using (var image = System.Drawing.Image.FromStream(new MemoryStream(filebytes)))
            using (var memImage = new Bitmap(image, image.Width, image.Height))
            {
                ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
                Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, newQuality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                using (var ms = new MemoryStream())
                {
                    memImage.Save(ms, myImageCodecInfo, myEncoderParameters);
                    return ms.ToArray();
                }
            }
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in encoders)
                if (ici.MimeType == mimeType) return ici;

            return null;
        }

        #endregion

        //public string convertFilenametoPDF(string name)
        //{
        //    return System.IO.Path.ChangeExtension(name, ".pdf");
        //}

        //public ConvertedAttachmentProps ConvertImagetoPdf(string filename, byte[] filebytes)
        //{
        //    var convertedAttachmentProps = new ConvertedAttachmentProps();
        //    var spltfn = filename.Split('.');
        //    var ext = spltfn[spltfn.Length - 1];

        //    if (ext.ToLower() == "pdf")
        //    {
        //        convertedAttachmentProps.Filename = filename;
        //        convertedAttachmentProps.PDFcontent = filebytes;
        //        return convertedAttachmentProps;
        //    }
        //    using (var imageStream = new MemoryStream(filebytes))
        //    {
        //        var imageData = ImageDataFactory.Create(filebytes);
        //        var image = new iText.Layout.Element.Image(imageData);
        //        var pageSize = new PageSize(imageData.GetWidth(), imageData.GetHeight());
        //        using (var outputStream = new MemoryStream())
        //        {
        //            using (var pdfWriter = new PdfWriter(outputStream))
        //            {
        //                using (var pdfDocument = new PdfDocument(pdfWriter))
        //                {
        //                    var document = new Document(pdfDocument, pageSize);
        //                    document.Add(image);
        //                    document.Close();
        //                }
        //            }
        //            convertedAttachmentProps.PDFcontent = outputStream.ToArray();
        //            convertedAttachmentProps.Filename = convertFilenametoPDF(filename);
        //        }
        //    }
        //    return convertedAttachmentProps;
        //}

        //public byte[] MergePdf(List<byte[]> files)
        //{
        //    try
        //    {
        //        using (var outputStream = new MemoryStream())
        //        {
        //            using (var pdfWriter = new PdfWriter(outputStream))
        //            {
        //                using (var pdfDocument = new PdfDocument(pdfWriter))
        //                {
        //                    var pdfMerger = new PdfMerger(pdfDocument);
        //                    foreach (var file in files)
        //                    {
        //                        using (var pdfReader = new PdfReader(new MemoryStream(file)))
        //                        using (var pdfSourceDocument = new PdfDocument(pdfReader))
        //                        {
        //                            pdfMerger.Merge(pdfSourceDocument, 1, pdfSourceDocument.GetNumberOfPages());
        //                        }
        //                    }
        //                }
        //            }
        //            return outputStream.ToArray();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private static byte[] CompressImage(byte[] fileBytes, int newQuality)
        //{
        //    using (var image = Image.Load(fileBytes))
        //    {
        //        var encoder = new JpegEncoder { Quality = newQuality };
        //        using (var outputStream = new MemoryStream())
        //        {
        //            image.Save(outputStream, encoder);
        //            return outputStream.ToArray();
        //        }
        //    }
        //}




        #region Unused

        //public byte[] MergePDF(List<byte[]> files)
        //{
        //    try
        //    {
        //        List<PdfReader> readerList = new List<PdfReader>();
        //        iTextSharp.text.Rectangle rectangle = null;
        //        foreach (byte[] file in files)
        //        {
        //            PdfReader pdfReader = new PdfReader(file);
        //            readerList.Add(pdfReader);
        //        }
        //        rectangle = readerList[0].GetPageSize(1);
        //        Document document = new Document(rectangle, 0, 0, 0, 0);
        //        MemoryStream outfutbase64 = new MemoryStream();
        //        PdfWriter writer = PdfWriter.GetInstance(document, outfutbase64);
        //        document.Open();
        //        foreach (PdfReader reader in readerList)
        //        {
        //            for (int i = 1; i <= reader.NumberOfPages; i++)
        //            {
        //                PdfImportedPage page = writer.GetImportedPage(reader, i);

        //                document.Add(iTextSharp.text.Image.GetInstance(page));
        //            }
        //        }
        //        document.Close();
        //        byte[] outbiteFile = outfutbase64.ToArray();
        //        return outbiteFile;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}


        //public ConvertedAttachmentProps ConvertImagetoPDF(string filename, byte[] filebytes)
        //{
        //    ConvertedAttachmentProps convertedAttachmentProps = new ConvertedAttachmentProps();
        //    var spltfn = filename.Split('.');
        //    var ext = spltfn[spltfn.Length - 1];
        //    if (ext.ToLower() == "pdf")
        //    {
        //        convertedAttachmentProps.Filename = filename;
        //        convertedAttachmentProps.PDFcontent = filebytes;
        //        return convertedAttachmentProps;
        //    }
        //    else
        //    {
        //        var fileSize = filebytes.Length / (1024 * 1024);
        //        if (fileSize < 1)
        //        {
        //            iTextSharp.text.Rectangle pageSize = null;
        //            MemoryStream fs = new MemoryStream(filebytes);
        //            using (var srcImage = new Bitmap(fs))
        //            {
        //                pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
        //            }

        //            using (var ms = new MemoryStream())
        //            {
        //                var document = new Document(pageSize, 0, 0, 0, 0);
        //                PdfWriter.GetInstance(document, ms).SetFullCompression();
        //                document.Open();
        //                var image = iTextSharp.text.Image.GetInstance(filebytes);
        //                document.Add(image);
        //                document.Close();
        //                convertedAttachmentProps.PDFcontent = ms.ToArray();
        //                convertedAttachmentProps.Filename = convertFilenametoPDF(filename);
        //            }
        //            return convertedAttachmentProps;
        //        }
        //        else
        //        {
        //            iTextSharp.text.Rectangle pageSize = null;
        //            MemoryStream fs = new MemoryStream(filebytes);
        //            using (var srcImage = new Bitmap(fs))
        //            {
        //                pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
        //            }

        //            using (var ms = new MemoryStream())
        //            {
        //                var document = new Document(pageSize, 0, 0, 0, 0);
        //                PdfWriter.GetInstance(document, ms).SetFullCompression();
        //                document.Open();
        //                iTextSharp.text.Image image;
        //                image = iTextSharp.text.Image.GetInstance(compressImage(filebytes, 15));
        //                image.SetAbsolutePosition(0, 0);
        //                image.ScaleAbsoluteHeight(pageSize.Height);
        //                image.ScaleAbsoluteWidth(pageSize.Width);
        //                document.Add(image);
        //                document.Close();
        //                convertedAttachmentProps.PDFcontent = ms.ToArray();
        //                convertedAttachmentProps.Filename = convertFilenametoPDF(filename);
        //            }

        //            return convertedAttachmentProps;
        //        }
        //    }
        //}


        //private static byte[] compressImage(byte[] filebytes, int newQuality)
        //{
        //    using (System.Drawing.Image image = System.Drawing.Image.FromStream(new MemoryStream(filebytes)))
        //    using (System.Drawing.Image memImage = new Bitmap(image, image.Width, image.Height))
        //    {
        //        ImageCodecInfo myImageCodecInfo;
        //        Encoder myEncoder;
        //        EncoderParameter myEncoderParameter;
        //        EncoderParameters myEncoderParameters;
        //        myImageCodecInfo = GetEncoderInfo("image/jpeg");
        //        myEncoder = Encoder.Quality;
        //        myEncoderParameters = new EncoderParameters(1);
        //        myEncoderParameter = new EncoderParameter(myEncoder, newQuality);
        //        myEncoderParameters.Param[0] = myEncoderParameter;
        //        MemoryStream ms = new MemoryStream();
        //        memImage.Save(ms, myImageCodecInfo, myEncoderParameters);
        //        return ms.ToArray();
        //    }
        //}

        //private static ImageCodecInfo GetEncoderInfo(String mimeType)
        //{
        //    ImageCodecInfo[] encoders;
        //    encoders = ImageCodecInfo.GetImageEncoders();
        //    foreach (ImageCodecInfo ici in encoders)
        //        if (ici.MimeType == mimeType) return ici;

        //    return null;
        //}

        #endregion
    }
}
