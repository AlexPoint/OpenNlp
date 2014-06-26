using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using Netron.Lithium;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Util;
using System.Configuration;

namespace ParseTree
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : Form
	{
		private IContainer _components;
		private TextBox _txtInput;
		private LithiumControl _lithiumControl;
		private Button _btnParse;

		private readonly string _modelPath;

		private EnglishTreebankParser _parser;
		private Parse _parse;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			_lithiumControl.Width = this.ClientRectangle.Width;
			_lithiumControl.Height = this.ClientRectangle.Height - _lithiumControl.Top;

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
			this._lithiumControl = new Netron.Lithium.LithiumControl();
			this._txtInput = new System.Windows.Forms.TextBox();
			this._btnParse = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _lithiumControl
			// 
			this._lithiumControl.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this._lithiumControl.AutoScroll = true;
			this._lithiumControl.AutoScrollMinSize = new System.Drawing.Size(-2147483547, -2147483547);
			this._lithiumControl.BackColor = System.Drawing.SystemColors.Window;
			this._lithiumControl.BranchHeight = 70;
			this._lithiumControl.ConnectionType = Netron.Lithium.ConnectionType.Default;
			this._lithiumControl.LayoutDirection = Netron.Lithium.TreeDirection.Vertical;
			this._lithiumControl.LayoutEnabled = true;
			this._lithiumControl.Location = new System.Drawing.Point(0, 48);
			this._lithiumControl.Name = "_lithiumControl";
			this._lithiumControl.Size = new System.Drawing.Size(200, 352);
			this._lithiumControl.TabIndex = 0;
			this._lithiumControl.Text = "_lithiumControl";
			this._lithiumControl.WordSpacing = 20;
			// 
			// _txtInput
			// 
			this._txtInput.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this._txtInput.Location = new System.Drawing.Point(8, 16);
			this._txtInput.Name = "_txtInput";
			this._txtInput.Size = new System.Drawing.Size(568, 20);
			this._txtInput.TabIndex = 1;
			this._txtInput.Text = "A rare black squirrel has become a regular visitor to a suburban garden.";
			this._txtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInput_KeyPress);
			// 
			// _btnParse
			// 
			this._btnParse.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			this._btnParse.Location = new System.Drawing.Point(584, 16);
			this._btnParse.Name = "_btnParse";
			this._btnParse.TabIndex = 2;
			this._btnParse.Text = "Parse";
			this._btnParse.Click += new System.EventHandler(this.btnParse_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(664, 478);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this._btnParse,
																		  this._txtInput,
																		  this._lithiumControl});
			this.Name = "Form1";
			this.Text = "OpenNLP Parser Demo";
			this.ResumeLayout(false);

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

		

		private void btnParse_Click(object sender, EventArgs e)
		{
			ShowParse();						
		}

		private void AddChildNodes(ShapeBase currentShape, IEnumerable<Parse> childParses)
		{
			foreach (Parse childParse in childParses)
			{
				// if this is not a token node (token node = one of the words of the sentence)
				if (childParse.Type != MaximumEntropyParser.TokenNode)
				{
					ShapeBase childShape = currentShape.AddChild(childParse.Type);
					if (childParse.IsPosTag)
					{
						childShape.ShapeColor = Color.DarkGoldenrod;
					}
					else
					{
						childShape.ShapeColor = Color.SteelBlue;
					}
					AddChildNodes(childShape, childParse.GetChildren());
					childShape.Expand();
				}
				else
				{
					Span parseSpan = childParse.Span;
					string token = childParse.Text.Substring(parseSpan.Start, (parseSpan.End) - (parseSpan.Start));
					ShapeBase childShape = currentShape.AddChild(token);
					childShape.ShapeColor = Color.Ivory;
				}
			}
		}

		private void ShowParse()
		{
			if (_txtInput.Text.Length == 0)
			{
				return;
			}

			//prepare the UI
			_txtInput.Enabled = false;
			_btnParse.Enabled = false;
			this.Cursor = Cursors.WaitCursor;

			_lithiumControl.NewDiagram();

			//do the parsing
			if (_parser == null)
			{
				_parser = new EnglishTreebankParser(_modelPath, true, false);
			}
			_parse = _parser.DoParse(_txtInput.Text);
			
			if (_parse.Type == MaximumEntropyParser.TopNode)
			{
				_parse = _parse.GetChildren()[0];
			}

			//display the parse result
			ShapeBase root = this._lithiumControl.Root;
			root.Text = _parse.Type;
			root.Visible = true;

			AddChildNodes(root, _parse.GetChildren());
			root.Expand();

			this._lithiumControl.DrawTree();

			//restore the UI
			this.Cursor = Cursors.Default;
			_txtInput.Enabled = true;
			_btnParse.Enabled = true;
		}

		private void txtInput_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ((int)e.KeyChar == (int)Keys.Enter)
			{
				ShowParse();
			}
		}

	}
}
