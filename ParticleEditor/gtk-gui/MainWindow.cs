
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	private global::Gtk.Action FileAction;
	private global::Gtk.Action MenuAction;
	private global::Gtk.Action FileAction1;
	private global::Gtk.Action FileAction2;
	private global::Gtk.VBox vbox3;
	private global::Gtk.MenuBar menubar7;
	private global::Gtk.HPaned hpaned2;
	private global::Gtk.VPaned vpaned5;
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	private global::Gtk.TreeView treeview2;
	private global::Gtk.Fixed fixed6;
	private global::Gtk.VPaned vpaned4;
	private global::Gtk.GLWidget glwidget2;
	private global::Gtk.DrawingArea drawingarea1;
	private global::Gtk.Statusbar statusbar5;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.FileAction = new global::Gtk.Action ("FileAction", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction, null);
		this.MenuAction = new global::Gtk.Action ("MenuAction", global::Mono.Unix.Catalog.GetString ("Menu"), null, null);
		this.MenuAction.ShortLabel = global::Mono.Unix.Catalog.GetString ("Menu");
		w1.Add (this.MenuAction, null);
		this.FileAction1 = new global::Gtk.Action ("FileAction1", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction1.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction1, null);
		this.FileAction2 = new global::Gtk.Action ("FileAction2", global::Mono.Unix.Catalog.GetString ("File"), null, null);
		this.FileAction2.ShortLabel = global::Mono.Unix.Catalog.GetString ("File");
		w1.Add (this.FileAction2, null);
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.UIManager.AddUiFromString ("<ui><menubar name=\'menubar7\'><menu name=\'FileAction2\' action=\'FileAction2\'/></men" +
		"ubar></ui>");
		this.menubar7 = ((global::Gtk.MenuBar)(this.UIManager.GetWidget ("/menubar7")));
		this.menubar7.Name = "menubar7";
		this.vbox3.Add (this.menubar7);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.menubar7]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hpaned2 = new global::Gtk.HPaned ();
		this.hpaned2.CanFocus = true;
		this.hpaned2.Name = "hpaned2";
		this.hpaned2.Position = 142;
		// Container child hpaned2.Gtk.Paned+PanedChild
		this.vpaned5 = new global::Gtk.VPaned ();
		this.vpaned5.CanFocus = true;
		this.vpaned5.Name = "vpaned5";
		this.vpaned5.Position = 212;
		// Container child vpaned5.Gtk.Paned+PanedChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.treeview2 = new global::Gtk.TreeView ();
		this.treeview2.CanFocus = true;
		this.treeview2.Name = "treeview2";
		this.GtkScrolledWindow.Add (this.treeview2);
		this.vpaned5.Add (this.GtkScrolledWindow);
		global::Gtk.Paned.PanedChild w4 = ((global::Gtk.Paned.PanedChild)(this.vpaned5 [this.GtkScrolledWindow]));
		w4.Resize = false;
		// Container child vpaned5.Gtk.Paned+PanedChild
		this.fixed6 = new global::Gtk.Fixed ();
		this.fixed6.Name = "fixed6";
		this.fixed6.HasWindow = false;
		this.vpaned5.Add (this.fixed6);
		this.hpaned2.Add (this.vpaned5);
		global::Gtk.Paned.PanedChild w6 = ((global::Gtk.Paned.PanedChild)(this.hpaned2 [this.vpaned5]));
		w6.Resize = false;
		// Container child hpaned2.Gtk.Paned+PanedChild
		this.vpaned4 = new global::Gtk.VPaned ();
		this.vpaned4.CanFocus = true;
		this.vpaned4.Name = "vpaned4";
		this.vpaned4.Position = 333;
		// Container child vpaned4.Gtk.Paned+PanedChild
		this.glwidget2 = new global::Gtk.GLWidget ();
		this.glwidget2.Name = "glwidget2";
		this.glwidget2.SingleBuffer = false;
		this.glwidget2.ColorBPP = 0;
		this.glwidget2.AccumulatorBPP = 0;
		this.glwidget2.DepthBPP = 0;
		this.glwidget2.StencilBPP = 0;
		this.glwidget2.Samples = 0;
		this.glwidget2.Stereo = false;
		this.glwidget2.GlVersionMajor = 0;
		this.glwidget2.GlVersionMinor = 0;
		this.vpaned4.Add (this.glwidget2);
		global::Gtk.Paned.PanedChild w7 = ((global::Gtk.Paned.PanedChild)(this.vpaned4 [this.glwidget2]));
		w7.Resize = false;
		// Container child vpaned4.Gtk.Paned+PanedChild
		this.drawingarea1 = new global::Gtk.DrawingArea ();
		this.drawingarea1.Name = "drawingarea1";
		this.vpaned4.Add (this.drawingarea1);
		this.hpaned2.Add (this.vpaned4);
		this.vbox3.Add (this.hpaned2);
		global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hpaned2]));
		w10.Position = 1;
		// Container child vbox3.Gtk.Box+BoxChild
		this.statusbar5 = new global::Gtk.Statusbar ();
		this.statusbar5.Name = "statusbar5";
		this.statusbar5.Spacing = 6;
		this.vbox3.Add (this.statusbar5);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.statusbar5]));
		w11.Position = 2;
		w11.Expand = false;
		w11.Fill = false;
		this.Add (this.vbox3);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 937;
		this.DefaultHeight = 526;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.glwidget2.RenderFrame += new global::System.EventHandler (this.OnGlwidget2RenderFrame);
	}
}
