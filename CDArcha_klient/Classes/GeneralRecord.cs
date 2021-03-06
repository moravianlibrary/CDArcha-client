using System.Linq;
using System.Collections.Generic;
using System;
using System.Windows.Media.Imaging;

namespace CDArcha_klient.Classes
{
    public abstract class GeneralRecord
    {
        /// <summary>Type of identifier, that is used for retrieval of metadata</summary>
        public IdentifierType IdentifierType { get; set; }

        /// <summary>Original value of searched identifier</summary>
        public string IdentifierValue { get; set; }

        /// <summary>Barcode of processed unit</summary>
        public string Barcode { get; set; }

        /// <summary>ČNB identifier of processed record</summary>
        public string Cnb { get; set; }

        /// <summary>OCLC number of processed record</summary>
        public string Oclc { get; set; }

        /// <summary>EAN of processed record</summary>
        public string Ean { get; set; }

        /// <summary>ISMN of processed record</summary>
        public string Ismn { get; set; }

        /// <summary>URN:NBN identifier of processed record</summary>
        public string Urn { get; set; }

        /// <summary>Title of processed record</summary>
        public string Title { get; set; }

        /// <summary>Authors of processed record</summary>
        public string Authors { get; set; }

        /// <summary>Publish year of processed record</summary>
        public string Year { get; set; }

        /// <summary>Publish year of particular processed unit</summary>
        public string PartYear { get; set; }

        /// <summary>Number of particular unit</summary>
        public string PartNo { get; set; }

        /// <summary>Name of particular unit (monography)</summary>
        public string PartName { get; set; }

        /// <summary>Custom identifier, it should be sysno (sigla is automatically attached)</summary>
        public string Custom { get; set; }

        /// <summary> Download link of cover from obalkyknih for this record </summary>
        public string OriginalCoverImageLink { get; set; }

        /// <summary> Download link of toc pdf from obalkyknih for this record </summary>
        public string OriginalTocPdfLink { get; set; }

        /// <summary> Download link for thumbnail of toc pfd from obalkyknih for this record </summary>
        public string OriginalTocThumbnailLink { get; set; }

        /// <summary> Authority ID and personal name key-value list </summary>
        public Dictionary<string, string> AuthList { get; set; }

        /// <summary>Imports title, authors, year and identifiers of union record from metadata</summary>
        /// <param name="metadata">Metadata of union record</param>
        public virtual void ImportFromMetadata(Metadata metadata)
        {
            // Title
            this.Title = ParseTitle(metadata);
            //Authors
            this.Authors = ParseAuthors(metadata);
            //Year
            this.Year = ParseYear(metadata);

            // Parse CNB
            this.Cnb = ParseIdentifier(metadata, Settings.MetadataCnbField).FirstOrDefault();
            // Parse UPC
            this.Ean = ParseIdentifier(metadata, Settings.MetadataUpcField).FirstOrDefault();
            // Parse EAN
            this.Ean = ParseIdentifier(metadata, Settings.MetadataEanField).FirstOrDefault();
            // Parse ISMN
            this.Ismn = ParseIdentifier(metadata, Settings.MetadataIsmnField).FirstOrDefault();
            // Parse OCLC
            this.Oclc = ParseIdentifier(metadata, Settings.MetadataOclcField).FirstOrDefault();
            // Custom ID
            this.Custom = metadata.Sysno;
        }

        /// <summary>Parses record title from all metadata fields as defined in Settings</summary>
        /// <param name="metadata">Metadata of record</param>
        /// <returns>Title parsed from metadata fields concatenated into single string or null</returns>
        protected string ParseTitle(Metadata metadata)
        {
            List<string> titleResults = new List<string>();
            foreach (var settingsField in Settings.MetadataTitleFields)
            {
                titleResults.AddRange(
                    metadata.VariableFields.Where(varField => settingsField.Key.ToString("D3").Equals(varField.TagName))
                                           .SelectMany(varField => varField.Subfields)
                                           .Where(subfield => settingsField.Value.Any(settingsValue => settingsValue.ToString().Equals(subfield.Key)))
                                           .Select(subfield => subfield.Value.TrimEnd('/', ' '))
                                     );
            }
            if (titleResults != null && titleResults.Count > 0)
            {
                return string.Join(" ", titleResults);
            }
            return null;
        }

        /// <summary>Parses record authors from all metadata fields as defined in Settings</summary>
        /// <param name="metadata">Metadata of record</param>
        /// <returns>Authors parsed from metadata fields concatenated into single string separated by coma or null</returns>
        protected string ParseAuthors(Metadata metadata)
        {
            List<string> authorResults = new List<string>();
            List<string> authorOutput = new List<string>();
            String lastAuthorName = "<neznamy autor>";
            this.AuthList = new Dictionary<string,string>();

            foreach (var settingsField in Settings.MetadataAuthorFields)
            {
                authorResults.Clear();
                authorResults.AddRange(metadata.VariableFields.Where(varField => settingsField.Key.ToString("D3").Equals(varField.TagName))
                                .SelectMany(varField => varField.Subfields)
                                .Where(subfield => settingsField.Value.Any(settingsValue => settingsValue.ToString().Equals(subfield.Key)))
                                .Select(subfield => subfield.Key + subfield.Value.TrimEnd('/', ' ', ',')));  // first char = subtag, continues by value
                for (int i = 0; i < authorResults.Count(); i++)
                {
                    var subtag = authorResults[i].Substring(0, 1);
                    var authorPartNames = authorResults[i].Substring(1).Split(',');
                    if (authorPartNames.Length > 1)
                    {
                        var tmp = authorPartNames[0];
                        authorPartNames[0] = authorPartNames[1];
                        authorPartNames[1] = tmp;
                    }
                    
                    if (subtag != "7")
                    {
                        var joinedAuthorName = string.Join(" ", authorPartNames);
                        authorOutput.Add(joinedAuthorName);
                        lastAuthorName = joinedAuthorName;
                    }
                    else
                    {
                        String authId = authorResults[i].Substring(1);
                        if (!this.AuthList.ContainsKey(authId))
                        {
                            this.AuthList.Add(authId, lastAuthorName);
                        }
                    }
                }
            }
            if (authorOutput != null && authorOutput.Count > 0)
            {
                return string.Join(", ", authorOutput);
            }
            return null;
        }

        /// <summary>Parse record publish year from single metadata field defined in Settings</summary>
        /// <param name="metadata">Metadata of record</param>
        /// <returns>Publish year</returns>
        protected string ParseYear(Metadata metadata)
        {
            // AACR2
            string yearAacr2 = metadata.VariableFields.Where(vf => Settings.MetadataPublishYearField.Item1.ToString("D3").Equals(vf.TagName))
                                                                           .SelectMany(vf => vf.Subfields)
                                                                           .Where(subf => Settings.MetadataPublishYearField.Item2.ToString().Equals(subf.Key))
                                                                           .Select(subf => subf.Value).FirstOrDefault();
            // RDA
            string yearRda = metadata.VariableFields.Where(  vf => Settings.MetadataPublishYearFieldRDA.Item1.ToString("D3").Equals(vf.TagName))
                                                                           .SelectMany(vf => vf.Subfields)
                                                                           .Where(subf => Settings.MetadataPublishYearFieldRDA.Item2.ToString().Equals(subf.Key))
                                                                           .Select(subf => subf.Value).FirstOrDefault();
            return yearRda ?? yearAacr2;
        }

        /// <summary>Parse particular record identifiers defined by metadataSettings from metadata</summary>
        /// <param name="metadata">Metadata of record</param>
        /// <param name="metadataSettings">Marc fields IDs of particular identifier from Settings</param>
        /// <returns>Enumeration of all found identifiers as string values</returns>
        protected IEnumerable<string> ParseIdentifier(Metadata metadata, Tuple<int, char, char?, char?> metadataSettings)
        {
            var tmp = metadata.VariableFields.Where(vf => metadataSettings.Item1.ToString("D3").Equals(vf.TagName)
                                                              && (
                                                                    (metadataSettings.Item3 == null)
                                                                 || (metadataSettings.Item3.ToString().Equals(vf.Indicator1))
                                                                 ) 
                                                              && (
                                                                    (metadataSettings.Item4 == null)
                                                                 || (metadataSettings.Item4.ToString().Equals(vf.Indicator2))
                                                                 )
                                                       );
                                                var tmp1 = tmp .SelectMany(vf => vf.Subfields);
                                                var tmp2 = tmp1.Where(subf => metadataSettings.Item2.ToString().Equals(subf.Key));
                                           var tmp3 = tmp2      .Select(subf => subf.Value);
                                           return tmp3;
        }
    }
}
