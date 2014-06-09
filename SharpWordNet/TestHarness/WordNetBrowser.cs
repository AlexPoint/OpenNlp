using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace TestHarness
{
	/// <summary>
	/// Summary description for WordNetBrowser.
	/// </summary>
	public class WordNetBrowser : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtResults;
		private System.Windows.Forms.Button btnGo;
		private System.Windows.Forms.TextBox txtWord;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Button btnHypernyms;

		private SharpWordNet.WordNetEngine moEngine;

		public WordNetBrowser()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			moEngine = new SharpWordNet.DataFileEngine(@"C:\Program Files\WordNet\2.1\dict");
			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
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
            this.txtResults = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtWord = new System.Windows.Forms.TextBox();
            this.btnHypernyms = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Location = new System.Drawing.Point(16, 80);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResults.Size = new System.Drawing.Size(500, 344);
            this.txtResults.TabIndex = 2;
            this.txtResults.WordWrap = false;
            // 
            // btnGo
            // 
            this.btnGo.Location = new System.Drawing.Point(440, 16);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 23);
            this.btnGo.TabIndex = 1;
            this.btnGo.Text = "Go";
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // txtWord
            // 
            this.txtWord.Location = new System.Drawing.Point(16, 16);
            this.txtWord.Name = "txtWord";
            this.txtWord.Size = new System.Drawing.Size(384, 20);
            this.txtWord.TabIndex = 0;
            // 
            // btnHypernyms
            // 
            this.btnHypernyms.Location = new System.Drawing.Point(440, 48);
            this.btnHypernyms.Name = "btnHypernyms";
            this.btnHypernyms.Size = new System.Drawing.Size(75, 23);
            this.btnHypernyms.TabIndex = 3;
            this.btnHypernyms.Text = "Hypernyms";
            this.btnHypernyms.Click += new System.EventHandler(this.btnHypernyms_Click);
            // 
            // WordNetBrowser
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(528, 438);
            this.Controls.Add(this.btnHypernyms);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtWord);
            this.Name = "WordNetBrowser";
            this.Text = "WordNet Browser";
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
			Application.Run(new WordNetBrowser());
		}

		private void btnGo_Click(object sender, System.EventArgs e)
		{
			System.Text.StringBuilder oOutput = new System.Text.StringBuilder();
			
			string sWord = txtWord.Text;

			string[] asPos = moEngine.GetPartsOfSpeech(sWord);
			foreach (string sPos in asPos)
			{
				oOutput.Append("Overview of ").Append(sPos).Append(" ").Append(sWord).Append("\r\n").Append("\r\n");

				int iCurrentSense = 0;
				foreach (SharpWordNet.Synset oSynset in moEngine.GetSynsets(sWord, sPos))
				{
					oOutput.Append(++iCurrentSense).Append(". ");
					oOutput.Append(oSynset.ToString()).Append("\r\n");
				}

				oOutput.Append("\r\n");
				oOutput.Append("The ").Append(sPos).Append(" ").Append(sWord).Append(" has ");
				oOutput.Append(iCurrentSense).Append(" sense");
				if (iCurrentSense != 1)
				{
					oOutput.Append("s");
				}
				int iTagSenseCount = moEngine.GetIndexWord(sWord, sPos).TagSenseCount;
				if (iTagSenseCount > 0)
				{
					oOutput.Append(" (first ").Append(iTagSenseCount).Append(" from tagged texts)");
				}
				else
				{
					oOutput.Append(" (none from tagged texts)");
				}
				oOutput.Append("\r\n");
				
//				WordNet.RelationType[] aoRelationTypes = moEngine.GetRelationTypes(sWord, sPos);
//				if (aoRelationTypes != null)
//				{
//					foreach (WordNet.RelationType oRelationType in aoRelationTypes)
//					{
//						oOutput.Append(oRelationType.Name).Append(" | ");
//					}
//					oOutput.Append("\r\n");
//				}
			}

			txtResults.Text = oOutput.ToString();
			
//			textBox1.Text = oEngine.GetSynset(txtWord.Text, "noun", 1).LexicographerFile;
		}

		private void btnHypernyms_Click(object sender, System.EventArgs e)
		{
			System.Text.StringBuilder oOutput = new System.Text.StringBuilder();
			string sWord = txtWord.Text;

			int iCurrentSense = 0;
			foreach (SharpWordNet.Synset oSynset in moEngine.GetSynsets(sWord, "noun"))
			{
				oOutput.Append("Sense ").Append(++iCurrentSense).Append("\r\n");
				GetHypernyms(oOutput, oSynset, 0);
				oOutput.Append("\r\n");
			}

			txtResults.Text = oOutput.ToString();
		}

		private void GetHypernyms(System.Text.StringBuilder output, SharpWordNet.Synset currentSynset, int level)
		{
			if (level > 0)
			{
				output.Append(new string('\t', level)).Append("=>");
			}
			output.Append(currentSynset.ToString()).Append("\r\n");
			for (int iCurrentRelation = 0; iCurrentRelation < currentSynset.RelationCount; iCurrentRelation++)
			{
				SharpWordNet.Relation oRelation = currentSynset.GetRelation(iCurrentRelation);
				if (oRelation.SynsetRelationType.Name == "Hypernym")
				{
					GetHypernyms(output, oRelation.TargetSynset, level + 1);
				}
			}
		}
	}
}
