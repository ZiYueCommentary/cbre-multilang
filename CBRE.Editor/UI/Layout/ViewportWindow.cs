﻿using System.Windows.Forms;

namespace CBRE.Editor.UI.Layout
{
    public partial class ViewportWindow : Form
    {
        public ViewportWindow(TableSplitConfiguration configuration)
        {
            InitializeComponent();

            SplitControl.Configuration = configuration;
        }

        public TableSplitControl TableSplitControl
        {
            get { return SplitControl; }
        }
    }
}
