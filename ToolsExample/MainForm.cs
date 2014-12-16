using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Text;
using System.Configuration;

namespace ToolsExample
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : Form
	{
		private Button _btnParse;
		private Button _btnPosTag;
		private Button _btnChunk;
		private Button _btnTokenize;
		private Button _btnNameFind;
        private Button _btnGender;
        private Button _btnSimilarity;
        private Button _btnCoreference;
		private TextBox _txtOut;
		private Button _btnSplit;
		private TextBox _txtIn;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private readonly Container _components = null;

		private readonly string _modelPath;

		private OpenNLP.Tools.SentenceDetect.MaximumEntropySentenceDetector _sentenceDetector;
		private OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer _tokenizer;
		private OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger _posTagger;
		private OpenNLP.Tools.Chunker.EnglishTreebankChunker _chunker;
        private OpenNLP.Tools.Parser.EnglishTreebankParser _parser;
		private OpenNLP.Tools.NameFind.EnglishNameFinder _nameFinder;
        private OpenNLP.Tools.Lang.English.TreebankLinker _coreferenceFinder;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_modelPath = ConfigurationManager.AppSettings["MaximumEntropyModelDirectory"];
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (_components != null) 
				{
					_components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._btnParse = new System.Windows.Forms.Button();
            this._btnPosTag = new System.Windows.Forms.Button();
            this._btnChunk = new System.Windows.Forms.Button();
            this._btnTokenize = new System.Windows.Forms.Button();
            this._btnNameFind = new System.Windows.Forms.Button();
            this._txtOut = new System.Windows.Forms.TextBox();
            this._btnSplit = new System.Windows.Forms.Button();
            this._txtIn = new System.Windows.Forms.TextBox();
            this._btnGender = new System.Windows.Forms.Button();
            this._btnSimilarity = new System.Windows.Forms.Button();
            this._btnCoreference = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnParse
            // 
            this._btnParse.Location = new System.Drawing.Point(360, 104);
            this._btnParse.Name = "_btnParse";
            this._btnParse.Size = new System.Drawing.Size(75, 23);
            this._btnParse.TabIndex = 21;
            this._btnParse.Text = "Parse";
            this._btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // btnPOSTag
            // 
            this._btnPosTag.Location = new System.Drawing.Point(184, 104);
            this._btnPosTag.Name = "_btnPosTag";
            this._btnPosTag.Size = new System.Drawing.Size(75, 23);
            this._btnPosTag.TabIndex = 20;
            this._btnPosTag.Text = "POS tag";
            this._btnPosTag.Click += new System.EventHandler(this.btnPOSTag_Click);
            // 
            // btnChunk
            // 
            this._btnChunk.Location = new System.Drawing.Point(272, 104);
            this._btnChunk.Name = "_btnChunk";
            this._btnChunk.Size = new System.Drawing.Size(75, 23);
            this._btnChunk.TabIndex = 19;
            this._btnChunk.Text = "Chunk";
            this._btnChunk.Click += new System.EventHandler(this.btnChunk_Click);
            // 
            // btnTokenize
            // 
            this._btnTokenize.Location = new System.Drawing.Point(96, 104);
            this._btnTokenize.Name = "_btnTokenize";
            this._btnTokenize.Size = new System.Drawing.Size(75, 23);
            this._btnTokenize.TabIndex = 18;
            this._btnTokenize.Text = "Tokenize";
            this._btnTokenize.Click += new System.EventHandler(this.btnTokenize_Click);
            // 
            // btnNameFind
            // 
            this._btnNameFind.Location = new System.Drawing.Point(448, 104);
            this._btnNameFind.Name = "_btnNameFind";
            this._btnNameFind.Size = new System.Drawing.Size(75, 23);
            this._btnNameFind.TabIndex = 16;
            this._btnNameFind.Text = "Find Names";
            this._btnNameFind.Click += new System.EventHandler(this.btnNameFind_Click);
            // 
            // txtOut
            // 
            this._txtOut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._txtOut.Location = new System.Drawing.Point(8, 136);
            this._txtOut.Multiline = true;
            this._txtOut.Name = "_txtOut";
            this._txtOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._txtOut.Size = new System.Drawing.Size(784, 400);
            this._txtOut.TabIndex = 15;
            this._txtOut.WordWrap = false;
            // 
            // btnSplit
            // 
            this._btnSplit.Location = new System.Drawing.Point(8, 104);
            this._btnSplit.Name = "_btnSplit";
            this._btnSplit.Size = new System.Drawing.Size(75, 23);
            this._btnSplit.TabIndex = 14;
            this._btnSplit.Text = "Split";
            this._btnSplit.Click += new System.EventHandler(this.btnSplit_Click);
            // 
            // txtIn
            // 
            this._txtIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._txtIn.Location = new System.Drawing.Point(8, 8);
            this._txtIn.Multiline = true;
            this._txtIn.Name = "_txtIn";
            this._txtIn.Size = new System.Drawing.Size(784, 88);
            this._txtIn.TabIndex = 13;
            this._txtIn.Text = resources.GetString("txtIn.Text");
            // 
            // btnGender
            // 
            this._btnGender.Location = new System.Drawing.Point(539, 104);
            this._btnGender.Name = "_btnGender";
            this._btnGender.Size = new System.Drawing.Size(75, 23);
            this._btnGender.TabIndex = 22;
            this._btnGender.Text = "Gender";
            this._btnGender.Click += new System.EventHandler(this.btnGender_Click);
            // 
            // btnSimilarity
            // 
            this._btnSimilarity.Location = new System.Drawing.Point(627, 104);
            this._btnSimilarity.Name = "_btnSimilarity";
            this._btnSimilarity.Size = new System.Drawing.Size(75, 23);
            this._btnSimilarity.TabIndex = 23;
            this._btnSimilarity.Text = "Similarity";
            this._btnSimilarity.Click += new System.EventHandler(this.btnSimilarity_Click);
            // 
            // btnCoreference
            // 
            this._btnCoreference.Location = new System.Drawing.Point(717, 104);
            this._btnCoreference.Name = "_btnCoreference";
            this._btnCoreference.Size = new System.Drawing.Size(75, 23);
            this._btnCoreference.TabIndex = 24;
            this._btnCoreference.Text = "Coreference";
            this._btnCoreference.Click += new System.EventHandler(this.btnCoreference_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(800, 542);
            this.Controls.Add(this._btnCoreference);
            this.Controls.Add(this._btnSimilarity);
            this.Controls.Add(this._btnGender);
            this.Controls.Add(this._btnParse);
            this.Controls.Add(this._btnPosTag);
            this.Controls.Add(this._btnChunk);
            this.Controls.Add(this._btnTokenize);
            this.Controls.Add(this._btnNameFind);
            this.Controls.Add(this._txtOut);
            this.Controls.Add(this._btnSplit);
            this.Controls.Add(this._txtIn);
            this.Name = "MainForm";
            this.Text = "OpenNLP Tools Example";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		#region Button click events

		private void btnSplit_Click(object sender, EventArgs e)
		{		
			string[] sentences = SplitSentences(_txtIn.Text);
			
			_txtOut.Text = string.Join("\r\n\r\n", sentences);
		}

		private void btnTokenize_Click(object sender, EventArgs e)
		{
			var output = new StringBuilder();

			string[] sentences = SplitSentences(_txtIn.Text);

			foreach(string sentence in sentences)
			{
				string[] tokens = TokenizeSentence(sentence);
				output.Append(string.Join(" | ", tokens)).Append("\r\n\r\n");
			}

			_txtOut.Text = output.ToString();
		}

		private void btnPOSTag_Click(object sender, EventArgs e)
		{
			var output = new StringBuilder();

			string[] sentences = SplitSentences(_txtIn.Text);

			foreach(string sentence in sentences)
			{
				string[] tokens = TokenizeSentence(sentence);
				string[] tags = PosTagTokens(tokens);

				for (int currentTag = 0; currentTag < tags.Length; currentTag++)
				{
					output.Append(tokens[currentTag]).Append("/").Append(tags[currentTag]).Append(" ");
				}

				output.Append("\r\n\r\n");
			}

			_txtOut.Text = output.ToString();
		}

		private void btnChunk_Click(object sender, EventArgs e)
		{
			var output = new StringBuilder();

			string[] sentences = SplitSentences(_txtIn.Text);

			foreach(string sentence in sentences)
			{
				string[] tokens = TokenizeSentence(sentence);
				string[] tags = PosTagTokens(tokens);

				output.Append(ChunkSentence(tokens, tags)).Append("\r\n\r\n");
			}

			_txtOut.Text = output.ToString();
		}

		private void btnParse_Click(object sender, EventArgs e)
		{
			var output = new StringBuilder();

			string[] sentences = SplitSentences(_txtIn.Text);

			foreach(string sentence in sentences)
			{
			    var parse = ParseSentence(sentence);
				output.Append(parse.Show()).Append("\r\n\r\n");
			}

			_txtOut.Text = output.ToString();
		}

		private void btnNameFind_Click(object sender, EventArgs e)
		{
			var output = new StringBuilder();

			string[] sentences = SplitSentences(_txtIn.Text);

			foreach(string sentence in sentences)
			{
                output.Append(FindNames(sentence)).Append("\r\n");
			}

			_txtOut.Text = output.ToString();
		}

		#endregion

		#region NLP methods

		private string[] SplitSentences(string paragraph)
		{
			if (_sentenceDetector == null)
			{
				_sentenceDetector = new OpenNLP.Tools.SentenceDetect.EnglishMaximumEntropySentenceDetector(_modelPath + "EnglishSD.nbin");
			}

			return _sentenceDetector.SentenceDetect(paragraph);
		}

		private string[] TokenizeSentence(string sentence)
		{
			if (_tokenizer == null)
			{
				_tokenizer = new OpenNLP.Tools.Tokenize.EnglishMaximumEntropyTokenizer(_modelPath + "EnglishTok.nbin");
			}

			return _tokenizer.Tokenize(sentence);
		}

		private string[] PosTagTokens(string[] tokens)
		{
			if (_posTagger == null)
			{
				_posTagger = new OpenNLP.Tools.PosTagger.EnglishMaximumEntropyPosTagger(_modelPath + "EnglishPOS.nbin", _modelPath + @"\Parser\tagdict");
			}

			return _posTagger.Tag(tokens);
		}

		private string ChunkSentence(string[] tokens, string[] tags)
		{
			if (_chunker == null)
			{
				_chunker = new OpenNLP.Tools.Chunker.EnglishTreebankChunker(_modelPath + "EnglishChunk.nbin");
			}
			
			return string.Join(" ", _chunker.GetChunks(tokens, tags));
		}

		private OpenNLP.Tools.Parser.Parse ParseSentence(string sentence)
		{
			if (_parser == null)
			{
				_parser = new OpenNLP.Tools.Parser.EnglishTreebankParser(_modelPath, true, false);
			}

			return _parser.DoParse(sentence);
		}

		private string FindNames(string sentence)
		{
			if (_nameFinder == null)
			{
				_nameFinder = new OpenNLP.Tools.NameFind.EnglishNameFinder(_modelPath + "namefind\\");
			}

			var models = new[] {"date", "location", "money", "organization", "percentage", "person", "time"};
			return _nameFinder.GetNames(models, sentence);
		}

        private string FindNames(OpenNLP.Tools.Parser.Parse sentenceParse)
        {
            if (_nameFinder == null)
            {
                _nameFinder = new OpenNLP.Tools.NameFind.EnglishNameFinder(_modelPath + "namefind\\");
            }

            var models = new[] { "date", "location", "money", "organization", "percentage", "person", "time" };
            return _nameFinder.GetNames(models, sentenceParse);
        }

        private string IdentifyCoreferents(IEnumerable<string> sentences)
        {
            if (_coreferenceFinder == null)
            {
                _coreferenceFinder = new OpenNLP.Tools.Lang.English.TreebankLinker(_modelPath + "coref");
            }

            var parsedSentences = new List<OpenNLP.Tools.Parser.Parse>();

            foreach (string sentence in sentences)
            {
                OpenNLP.Tools.Parser.Parse sentenceParse = ParseSentence(sentence);
                parsedSentences.Add(sentenceParse);
            }
            return _coreferenceFinder.GetCoreferenceParse(parsedSentences.ToArray());
        }

		#endregion

        private void btnGender_Click(object sender, EventArgs e)
        {
            StringBuilder output = new StringBuilder();

            string[] sentences = SplitSentences(_txtIn.Text);
            
            foreach (string sentence in sentences)
            {
                string[] tokens = TokenizeSentence(sentence);
                string[] tags = PosTagTokens(tokens);

                string posTaggedSentence = string.Empty;

                for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                {
                    posTaggedSentence += tokens[currentTag] + @"/" + tags[currentTag] + " ";
                }

                output.Append(posTaggedSentence);
                output.Append("\r\n");
                output.Append(OpenNLP.Tools.Coreference.Similarity.GenderModel.GenderMain(_modelPath + "coref\\gen", posTaggedSentence));
                output.Append("\r\n\r\n");
            }

            _txtOut.Text = output.ToString();
        }

        private void btnSimilarity_Click(object sender, EventArgs e)
        {
            var output = new StringBuilder();

            string[] sentences = SplitSentences(_txtIn.Text);

            foreach (string sentence in sentences)
            {
                string[] tokens = TokenizeSentence(sentence);
                string[] tags = PosTagTokens(tokens);

                string posTaggedSentence = string.Empty;

                for (int currentTag = 0; currentTag < tags.Length; currentTag++)
                {
                    posTaggedSentence += tokens[currentTag] + @"/" + tags[currentTag] + " ";
                }

                output.Append(posTaggedSentence);
                output.Append("\r\n");
                output.Append(OpenNLP.Tools.Coreference.Similarity.SimilarityModel.SimilarityMain(_modelPath + "coref\\sim", posTaggedSentence));
                output.Append("\r\n\r\n");
            }

            _txtOut.Text = output.ToString();
        }

        private void btnCoreference_Click(object sender, EventArgs e)
        {
            string[] sentences = SplitSentences(_txtIn.Text);

            _txtOut.Text = IdentifyCoreferents(sentences);
        }
		
	}
}
