using Soft3D4Net.Painter;
using Soft3D4Net.Utils;
using Soft3D4Net.Volume;
using Soft3D4Net.World;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Forms;

namespace Soft3D4Net.WinForms.Demo;

public partial class MainScreen : Form {

    public MainScreen() {
        InitializeComponent();

        // var v = VolumeFactory.HackyImportCollada("Models\\skull.dae").ToList();

        lstDemos.DataSource = new[] {
                new { display = "Skull", id = "skull" },
                new { display = "Parrot", id = "parrot" },
                new { display = "Elefant", id = "elefant" },
                new { display = "Teapot", id = "teapot" },
                new { display = "Juliet", id = "Juliet" },
                new { display = "Cubes", id = "cubes" },
                new { display = "Spheres", id = "spheres" },
                new { display = "Little town", id = "littletown" },
                new { display = "Town", id = "town" },
                new { display = "Big town", id = "bigtown" },
                new { display = "Cube", id = "cube" },
                new { display = "Big cube", id = "bigcube" },
                new { display = "Empty", id = "empty" },
            };

        lstDemos.ValueMember = "id";
        lstDemos.DisplayMember = "display";

        lstDemos.DoubleClick += LstDemos_DoubleClick;

        rdbNoneShading.Checked = panel3D1.Painter == null;
        rdbClassicShading.Checked = panel3D1.Painter is ClassicPainter;
        rdbFlatShading.Checked = panel3D1.Painter is FlatPainter;
        rdbGouraudShading.Checked = panel3D1.Painter is GouraudPainter;

        rdbNoneShading.CheckedChanged += (s, e) => { if(!((RadioButton)s).Checked) return; panel3D1.Painter = null; panel3D1.Invalidate(); };
        rdbClassicShading.CheckedChanged += (s, e) => { if(!((RadioButton)s).Checked) return; panel3D1.Painter = new ClassicPainter(); panel3D1.Invalidate(); };
        rdbFlatShading.CheckedChanged += (s, e) => { if(!((RadioButton)s).Checked) return; panel3D1.Painter = new FlatPainter(); panel3D1.Invalidate(); };
        rdbGouraudShading.CheckedChanged += (s, e) => { if(!((RadioButton)s).Checked) return; panel3D1.Painter = new GouraudPainter(); panel3D1.Invalidate(); };

        chkShowTriangles.Checked = panel3D1.RendererSettings.ShowTriangles;
        chkShowBackFacesCulling.Checked = panel3D1.RendererSettings.BackFaceCulling;
        chkShowXZGrid.Checked = panel3D1.RendererSettings.ShowXZGrid;
        chkShowAxes.Checked = panel3D1.RendererSettings.ShowAxes;

        chkShowTriangles.CheckedChanged += (s, e) => { panel3D1.RendererSettings.ShowTriangles = chkShowTriangles.Checked; panel3D1.Invalidate(); };
        chkShowBackFacesCulling.CheckedChanged += (s, e) => { panel3D1.RendererSettings.BackFaceCulling = chkShowBackFacesCulling.Checked; panel3D1.Invalidate(); };
        chkShowXZGrid.CheckedChanged += (s, e) => { panel3D1.RendererSettings.ShowXZGrid = chkShowXZGrid.Checked; panel3D1.Invalidate(); };
        chkShowAxes.CheckedChanged += (s, e) => { panel3D1.RendererSettings.ShowAxes = chkShowAxes.Checked; panel3D1.Invalidate(); };

        this.panel3D1.Scene = new Scene() {
            Projection = new FovPerspectiveProjection(40f * (float)Math.PI / 180f, .01f, 500f),
            Camera = new ArcBallCam(this.panel3D1) { Position = new Vector3(0, 0, -60) }
        };

        PrepareWorld("skull");
    }

    private void LstDemos_DoubleClick(object sender, EventArgs e) {
        PrepareWorld(lstDemos.SelectedValue as string);
    }

    void PrepareWorld(string id) {
        var world = new SimpleWorld();

        panel3D1.Scene.Camera.Position = new Vector3(0, 0, -60);

        switch(id) {
            case "skull":
                world.Volumes.AddRange(VolumeFactory.HackyImportCollada(@"models\skull.dae"));
                panel3D1.Scene.Camera.Position = new Vector3(0, 0, -5);
                break;

            case "parrot":
                world.Volumes.AddRange(VolumeFactory.HackyImportCollada(@"models\parrot.dae"));
                panel3D1.Scene.Camera.Position = new Vector3(0, 0, -500);
                break;

            case "teapot":
                world.Volumes.AddRange(VolumeFactory.HackyImportCollada(@"models\teapot.dae"));
                break;

            case "elefant":
                world.Volumes.AddRange(VolumeFactory.HackyImportCollada(@"models\elefant.dae"));
                panel3D1.Scene.Camera.Position = new Vector3(0, 0, -1500);
                panel3D1.Scene.Projection = new FovPerspectiveProjection(40f * (float)Math.PI / 180f, .01f, 65535f);
                break;

            case "Juliet":
                world.Volumes.AddRange(VolumeFactory.HackyImportCollada(@"models\Juliet.dae"));
                panel3D1.Scene.Camera.Position = new Vector3(0, 0, -500);
                break;

            case "empty":
                break;

            case "town": {
                    var d = 50; var s = 2;
                    for(var x = -d; x <= d; x += s)
                        for(var z = -d; z <= d; z += s) {
                            world.Volumes.Add(
                                new Cube() {
                                    Position = new Vector3(x, 0, z),
                                    // Scale = new Vector3(1, r.Next(1, 50), 1)
                                });
                        }
                    break;
                }

            case "littletown": {
                    var d = 10; var s = 2;
                    for(var x = -d; x <= d; x += s)
                        for(var z = -d; z <= d; z += s) {
                            world.Volumes.Add(
                                new Cube() {
                                    Position = new Vector3(x, 0, z),
                                    // Scale = new Vector3(1, r.Next(1, 50), 1)
                                });
                        }
                    break;
                }

            case "bigtown": {
                    var d = 200; var s = 2;
                    for(var x = -d; x <= d; x += s)
                        for(var z = -d; z <= d; z += s) {
                            world.Volumes.Add(
                                new Cube() {
                                    Position = new Vector3(x, 0, z),
                                    // Scale = new Vector3(1, r.Next(1, 50), 1)
                                });
                        }
                    break;
                }

            case "cube":
                world.Volumes.Add(new Cube());
                break;

            case "bigcube":
                world.Volumes.Add(new Cube() { Scale = new Vector3(100, 100, 100) });
                break;

            case "spheres": {
                    var d = 5; var s = 2;
                    for(var x = -d; x <= d; x += s)
                        for(var y = -d; y <= d; y += s)
                            for(var z = -d; z <= d; z += s) {
                                world.Volumes.Add(
                                    new IcoSphere(2) {
                                        Position = new Vector3(x, y, z)
                                    });
                            }
                    break;
                }

            case "cubes": {
                    var d = 20; var s = 2; var r = new Random();
                    for(var x = -d; x <= d; x += s)
                        for(var y = -d; y <= d; y += s)
                            for(var z = -d; z <= d; z += s) {
                                world.Volumes.Add(
                                    new Cube() {
                                        Position = new Vector3(x, y, z),
                                        Rotation = new Rotation3D(
                                            r.Next(-90, 90),
                                            r.Next(-90, 90),
                                            r.Next(-90, 90)).ToRad()
                                    });
                            }
                    break;
                }
        }

        world.LightSources.Add(new LightSource { Position = new Vector3(0, 0, 10) });

        this.panel3D1.Scene.World = world;
        this.panel3D1.Invalidate();
    }
}