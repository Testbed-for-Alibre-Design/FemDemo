Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Text
Imports System.Threading
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Entities
Imports devDept.Eyeshot.Triangulation
Imports devDept.Geometry
Imports devDept.Graphics
Imports devDept.Eyeshot.Fem
Imports Size = System.Drawing.Size
Imports System.Windows.Media
Imports devDept.Eyeshot.Translators

Partial Class Draw
    Public Shared Color As System.Drawing.Color = System.Drawing.Color.Black

    Public Shared fm As FemMesh

    Public Shared Sub Truss(viewportLayout As ViewportLayout)

        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        ' Show the legend
        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).ColorTable = Legend.RedToBlue9
            viewportLayout.GetLegends()(0).IsSlave = True
            viewportLayout.GetLegends()(0).ItemSize = New Size(10, 24)
            viewportLayout.GetLegends()(0).Visible = True
        End If

        ' Define Material
        Dim steel As Material = Material.StructuralSteel
        Dim brass As Material = Material.Brass

        '#Region "Mesh definition"

        ' Initialize FemMesh
        fm = New FemMesh(36, 102)

        ' Define vertices
        fm.Vertices(0) = New Node(0, 0, 0)
        fm.Vertices(1) = New Node(0.5, 0, 0)
        fm.Vertices(2) = New Node(0, 0.5, 0)

        fm.Vertices(3) = New Node(0, 0, 0.5)
        fm.Vertices(4) = New Node(0.5, 0, 0.5)
        fm.Vertices(5) = New Node(0, 0.5, 0.5)

        fm.Vertices(6) = New Node(0, 0, 1)
        fm.Vertices(7) = New Node(0.5, 0, 1)
        fm.Vertices(8) = New Node(0, 0.5, 1)

        fm.Vertices(9) = New Node(0, 0, 1.5)
        fm.Vertices(10) = New Node(0.5, 0, 1.5)
        fm.Vertices(11) = New Node(0, 0.5, 1.5)

        fm.Vertices(12) = New Node(0, 0, 2)
        fm.Vertices(13) = New Node(0.5, 0, 2)
        fm.Vertices(14) = New Node(0, 0.5, 2)

        fm.Vertices(15) = New Node(0, 0, 2.5)
        fm.Vertices(16) = New Node(0.5, 0, 2.5)
        fm.Vertices(17) = New Node(0, 0.5, 2.5)

        fm.Vertices(18) = New Node(0, 0, 3)
        'braccio
        fm.Vertices(19) = New Node(0.5, 0, 3)
        fm.Vertices(20) = New Node(0, 0.5, 3)

        fm.Vertices(21) = New Node(0, 0, 3.5)
        'angolo braccio
        fm.Vertices(22) = New Node(0.5, 0, 3.5)
        fm.Vertices(23) = New Node(0, 0.5, 3.5)
        'braccio
        fm.Vertices(24) = New Node(0, 0, 4)
        fm.Vertices(25) = New Node(0.5, 0, 4)
        fm.Vertices(26) = New Node(0, 0.5, 4)


        fm.Vertices(27) = New Node(-1.5, 0, 3.5)
        fm.Vertices(28) = New Node(-1.5, 0, 3)
        fm.Vertices(29) = New Node(-1.5, 0.5, 3.5)

        fm.Vertices(30) = New Node(-1, 0, 3.5)
        fm.Vertices(31) = New Node(-1, 0, 3)
        fm.Vertices(32) = New Node(-1, 0.5, 3.5)

        fm.Vertices(33) = New Node(-0.5, 0, 3.5)
        fm.Vertices(34) = New Node(-0.5, 0, 3)
        fm.Vertices(35) = New Node(-0.5, 0.5, 3.5)


        Const sectionArea As Double = 0.001

        ' Add elements

        'first segment
        fm.Elements(0) = New Truss(0, 1, steel, sectionArea)
        fm.Elements(1) = New Truss(1, 2, steel, sectionArea)
        fm.Elements(2) = New Truss(2, 0, steel, sectionArea)

        fm.Elements(3) = New Truss(0, 3, steel, sectionArea)
        fm.Elements(4) = New Truss(1, 4, steel, sectionArea)
        fm.Elements(5) = New Truss(2, 5, steel, sectionArea)

        fm.Elements(6) = New Truss(0, 4, brass, sectionArea)
        fm.Elements(7) = New Truss(0, 5, steel, sectionArea)
        fm.Elements(8) = New Truss(2, 4, steel, sectionArea)


        'second segment
        fm.Elements(9) = New Truss(3, 4, steel, sectionArea)
        fm.Elements(10) = New Truss(4, 5, steel, sectionArea)
        fm.Elements(11) = New Truss(5, 3, steel, sectionArea)

        fm.Elements(12) = New Truss(3, 6, steel, sectionArea)
        fm.Elements(13) = New Truss(4, 7, steel, sectionArea)
        fm.Elements(14) = New Truss(5, 8, steel, sectionArea)

        fm.Elements(15) = New Truss(3, 8, steel, sectionArea)
        fm.Elements(16) = New Truss(3, 7, steel, sectionArea)
        fm.Elements(17) = New Truss(4, 8, steel, sectionArea)


        'third segment
        fm.Elements(18) = New Truss(0 + 6, 1 + 6, steel, sectionArea)
        fm.Elements(19) = New Truss(1 + 6, 2 + 6, steel, sectionArea)
        fm.Elements(20) = New Truss(2 + 6, 0 + 6, steel, sectionArea)

        fm.Elements(21) = New Truss(0 + 6, 3 + 6, steel, sectionArea)
        fm.Elements(22) = New Truss(1 + 6, 4 + 6, steel, sectionArea)
        fm.Elements(23) = New Truss(2 + 6, 5 + 6, steel, sectionArea)

        fm.Elements(24) = New Truss(0 + 6, 4 + 6, steel, sectionArea)
        fm.Elements(25) = New Truss(0 + 6, 5 + 6, steel, sectionArea)
        fm.Elements(26) = New Truss(2 + 6, 4 + 6, steel, sectionArea)


        'fourth segment
        fm.Elements(27) = New Truss(3 + 6, 4 + 6, steel, sectionArea)
        fm.Elements(28) = New Truss(4 + 6, 5 + 6, steel, sectionArea)
        fm.Elements(29) = New Truss(5 + 6, 3 + 6, steel, sectionArea)

        fm.Elements(30) = New Truss(3 + 6, 6 + 6, steel, sectionArea)
        fm.Elements(31) = New Truss(4 + 6, 7 + 6, steel, sectionArea)
        fm.Elements(32) = New Truss(5 + 6, 8 + 6, steel, sectionArea)

        fm.Elements(33) = New Truss(3 + 6, 8 + 6, steel, sectionArea)
        fm.Elements(34) = New Truss(3 + 6, 7 + 6, steel, sectionArea)
        fm.Elements(35) = New Truss(4 + 6, 8 + 6, steel, sectionArea)


        'fifth segment
        fm.Elements(36) = New Truss(0 + 12, 1 + 12, steel, sectionArea)
        fm.Elements(37) = New Truss(1 + 12, 2 + 12, steel, sectionArea)
        fm.Elements(38) = New Truss(2 + 12, 0 + 12, steel, sectionArea)

        fm.Elements(39) = New Truss(0 + 12, 3 + 12, steel, sectionArea)
        fm.Elements(40) = New Truss(1 + 12, 4 + 12, steel, sectionArea)
        fm.Elements(41) = New Truss(2 + 12, 5 + 12, steel, sectionArea)

        fm.Elements(42) = New Truss(0 + 12, 4 + 12, steel, sectionArea)
        fm.Elements(43) = New Truss(0 + 12, 5 + 12, steel, sectionArea)
        fm.Elements(44) = New Truss(2 + 12, 4 + 12, steel, sectionArea)


        'sixth segment
        fm.Elements(45) = New Truss(3 + 12, 4 + 12, brass, sectionArea)
        fm.Elements(46) = New Truss(4 + 12, 5 + 12, steel, sectionArea)
        fm.Elements(47) = New Truss(5 + 12, 3 + 12, steel, sectionArea)

        fm.Elements(48) = New Truss(3 + 12, 6 + 12, steel, sectionArea)
        fm.Elements(49) = New Truss(4 + 12, 7 + 12, steel, sectionArea)
        fm.Elements(50) = New Truss(5 + 12, 8 + 12, steel, sectionArea)

        fm.Elements(51) = New Truss(3 + 12, 8 + 12, steel, sectionArea)
        fm.Elements(52) = New Truss(3 + 12, 7 + 12, steel, sectionArea)
        fm.Elements(53) = New Truss(4 + 12, 8 + 12, steel, sectionArea)


        'seventh segment
        fm.Elements(54) = New Truss(0 + 18, 1 + 18, steel, sectionArea)
        fm.Elements(55) = New Truss(1 + 18, 2 + 18, steel, sectionArea)
        fm.Elements(56) = New Truss(2 + 18, 0 + 18, steel, sectionArea)

        fm.Elements(57) = New Truss(0 + 18, 3 + 18, steel, sectionArea)
        fm.Elements(58) = New Truss(1 + 18, 4 + 18, steel, sectionArea)
        fm.Elements(59) = New Truss(2 + 18, 5 + 18, steel, sectionArea)

        fm.Elements(60) = New Truss(0 + 18, 4 + 18, steel, sectionArea)
        fm.Elements(61) = New Truss(0 + 18, 5 + 18, steel, sectionArea)
        fm.Elements(62) = New Truss(2 + 18, 4 + 18, steel, sectionArea)


        'eighth segment
        fm.Elements(63) = New Truss(3 + 18, 4 + 18, steel, sectionArea)
        fm.Elements(64) = New Truss(4 + 18, 5 + 18, steel, sectionArea)
        fm.Elements(65) = New Truss(5 + 18, 3 + 18, steel, sectionArea)

        fm.Elements(66) = New Truss(3 + 18, 6 + 18, steel, sectionArea)
        fm.Elements(67) = New Truss(4 + 18, 7 + 18, steel, sectionArea)
        fm.Elements(68) = New Truss(5 + 18, 8 + 18, steel, sectionArea)

        fm.Elements(69) = New Truss(3 + 18, 8 + 18, steel, sectionArea)
        fm.Elements(70) = New Truss(3 + 18, 7 + 18, steel, sectionArea)
        fm.Elements(71) = New Truss(4 + 18, 8 + 18, steel, sectionArea)


        'top
        fm.Elements(72) = New Truss(24, 25, steel, sectionArea)
        fm.Elements(73) = New Truss(25, 26, steel, sectionArea)
        fm.Elements(74) = New Truss(26, 24, steel, sectionArea)


        'first segment arm
        fm.Elements(75) = New Truss(0 + 27, 1 + 27, steel, sectionArea)
        fm.Elements(76) = New Truss(1 + 27, 2 + 27, steel, sectionArea)
        fm.Elements(77) = New Truss(2 + 27, 0 + 27, steel, sectionArea)

        fm.Elements(78) = New Truss(0 + 27, 3 + 27, steel, sectionArea)
        fm.Elements(79) = New Truss(1 + 27, 4 + 27, steel, sectionArea)
        fm.Elements(80) = New Truss(2 + 27, 5 + 27, steel, sectionArea)

        fm.Elements(81) = New Truss(0 + 27, 4 + 27, steel, sectionArea)
        fm.Elements(82) = New Truss(0 + 27, 5 + 27, steel, sectionArea)
        fm.Elements(83) = New Truss(2 + 27, 4 + 27, steel, sectionArea)

        'second segment arm
        fm.Elements(84) = New Truss(3 + 27, 4 + 27, steel, sectionArea)
        fm.Elements(85) = New Truss(4 + 27, 5 + 27, steel, sectionArea)
        fm.Elements(86) = New Truss(5 + 27, 3 + 27, steel, sectionArea)

        fm.Elements(87) = New Truss(3 + 27, 6 + 27, steel, sectionArea)
        fm.Elements(88) = New Truss(4 + 27, 7 + 27, steel, sectionArea)
        fm.Elements(89) = New Truss(5 + 27, 8 + 27, steel, sectionArea)

        fm.Elements(90) = New Truss(3 + 27, 8 + 27, steel, sectionArea)
        fm.Elements(91) = New Truss(3 + 27, 7 + 27, steel, sectionArea)
        fm.Elements(92) = New Truss(4 + 27, 8 + 27, steel, sectionArea)

        'third segment arm
        fm.Elements(93) = New Truss(0 + 33, 1 + 33, steel, sectionArea)
        fm.Elements(94) = New Truss(1 + 33, 2 + 33, steel, sectionArea)
        fm.Elements(95) = New Truss(2 + 33, 0 + 33, steel, sectionArea)

        fm.Elements(96) = New Truss(0 + 33, 21, steel, sectionArea)
        fm.Elements(97) = New Truss(1 + 33, 18, steel, sectionArea)
        fm.Elements(98) = New Truss(2 + 33, 23, steel, sectionArea)

        fm.Elements(99) = New Truss(0 + 33, 18, steel, sectionArea)
        fm.Elements(100) = New Truss(0 + 33, 23, steel, sectionArea)
        fm.Elements(101) = New Truss(2 + 33, 18, steel, sectionArea)

        '#End Region

        ' Add restraints
        DirectCast(fm.Vertices(0), Node).SetRestraint(True, True, True, 0, 0, 0)
        DirectCast(fm.Vertices(1), Node).SetRestraint(True, True, True, 0, 0, 0)
        DirectCast(fm.Vertices(2), Node).SetRestraint(True, True, True, 0, 0, 0)

        ' Add load
        DirectCast(fm.Vertices(27), Node).SetForce(0, 0, -0.5)
        DirectCast(fm.Vertices(29), Node).SetForce(0, 0, -0.5)

        ' Add FemMesh to entities collection
        viewportLayout.Entities.Add(fm)

        viewportLayout.SetView(viewType.Trimetric)
        viewportLayout.ZoomFit()

        Dim solver As New Solver(fm)

        ' Solve synchronously
        solver.DoWork()

        fm.PlotMode = FemMesh.plotType.Sx
        fm.ContourPlot = False
        fm.NodalAverages = False

        fm.ComputePlot(viewportLayout, viewportLayout.GetLegends()(0))

    End Sub

    Public Shared Sub Quad8(viewportLayout As ViewportLayout)

        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).Visible = True
        End If
        viewportLayout.ShowLoad = True
        viewportLayout.ShowRestraint = True

        Dim rl As New ReadLusas(devDept.DataSet.DataSet.GetStream("local_transform_2D.dat"))

        rl.DoWork()

        fm = DirectCast(rl.Entities(0), FemMesh)

        fm.PlotMode = FemMesh.plotType.VonMises

        viewportLayout.Entities.Add(fm)

        Dim solver As New Solver(fm)

        ' Solve asynchronously
        viewportLayout.StartWork(solver)

    End Sub

    Public Shared Sub Hex20(viewportLayout As ViewportLayout)

        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).Visible = True
        End If
        viewportLayout.ShowLoad = True
        viewportLayout.ShowRestraint = True

        Dim rl As New ReadLusas(devDept.DataSet.DataSet.GetStream("shear_load.dat"))

        rl.DoWork()

        fm = DirectCast(rl.Entities(0), FemMesh)

        fm.PlotMode = FemMesh.plotType.VonMises

        viewportLayout.Entities.Add(fm)

        Dim solver As New Solver(fm)

        ' Solve asynchronously
        viewportLayout.StartWork(solver)

    End Sub

    Public Shared Sub TShapedBeam(viewportLayout As ViewportLayout)
        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        ' Show the legend
        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).ColorTable = Legend.RedToBlue9
            viewportLayout.GetLegends()(0).IsSlave = True
            viewportLayout.GetLegends()(0).ItemSize = New Size(10, 24)
            viewportLayout.GetLegends()(0).Visible = True
        End If

        Dim n1 As New Node(0, 0, 0)
        Dim n2 As New Node(11, 0, 0)
        Dim n3 As New Node(11, 1, 0)
        Dim n4 As New Node(0, 1, 0)

        Dim nodes As New List(Of Point3D)() From { _
            n1, _
            n2, _
            n3, _
            n4 _
        }

        Dim material As New Material()
        material.Poisson = 0.3
        material.Young = 200000000000.0

        Dim quad4 As New Quad4(New List(Of Integer)() From { _
            0, _
            1, _
            2, _
            3 _
        }, material)

        fm = New FemMesh(nodes, New List(Of Element)() From { _
            quad4 _
        })

        fm.RefineElements(New List(Of Integer)() From { _
            0 _
        }, 11, 4)

        Dim items As New List(Of Integer)()
        For index As Integer = 0 To fm.Elements.Length - 1
            items.Add(index)
        Next

        fm.Extrude(items, New Vector3D(0, 0, 2), 2)

        fm.Regen(0.1)

        Dim boxMin As New Point3D(10, 0.5, 0)
        Dim boxMax As New Point3D(11, 1.2, 0)

        fm.Extrude(boxMin, boxMax, 0.1, New Vector3D(0, 0, -1))

        Dim boxMin1 As New Point3D(10, 0.5, 2)
        Dim boxMax1 As New Point3D(11, 1.2, 2)

        fm.Extrude(boxMin1, boxMax1, 0.1, New Vector3D(0, 0, 1))

        fm.ElevateElementOrder()

        fm.MergeNearbyNodes()

        fm.Regen(0.1)

        Dim boxMin2 As New Point3D(0, 0, 0)
        Dim boxMax2 As New Point3D(0, 1, 1)

        Dim plane As New Plane(boxMin2, New Point3D(0, 0.5, 1), boxMax2)

        fm.FixAllNodes(plane, 0.1)

        Dim node As Node = DirectCast(fm.Vertices(176), Node)
        node.SetForce(0, -5000, 0)

        node = DirectCast(fm.Vertices(607), Node)
        node.SetForce(0, -5000, 0)

        node = DirectCast(fm.Vertices(609), Node)
        node.SetForce(0, -5000, 0)

        node = DirectCast(fm.Vertices(610), Node)
        node.SetForce(0, -5000, 0)

        node = DirectCast(fm.Vertices(614), Node)
        node.SetForce(0, -5000, 0)

        node = DirectCast(fm.Vertices(617), Node)
        node.SetForce(0, -5000, 0)

        node = DirectCast(fm.Vertices(620), Node)
        node.SetForce(0, -5000, 0)

        viewportLayout.Entities.Add(fm)

        Dim solver As New Solver(fm)

        viewportLayout.StartWork(solver)

    End Sub

    Public Shared Sub CantileverBeam(viewportLayout As ViewportLayout)
        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        ' Show the legend
        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).ColorTable = Legend.RedToBlue9
            viewportLayout.GetLegends()(0).IsSlave = True
            viewportLayout.GetLegends()(0).ItemSize = New Size(10, 24)
            viewportLayout.GetLegends()(0).Visible = True
        End If

        fm = New RectangularFemMesh(0, -10.5, 100, 10)

        fm.RefineElements(20, 2)

        fm.Extrude(New Vector3D(0, 0, 1))

        fm.Extrude(Plane.ZX, 0.5, New Vector3D(0, 1, 0))

        fm.Extrude(New Point3D(0, -0.5, 1), New Point3D(100, +0.5, 1), 0.1, New Vector3D(0, 0, 18), 4)

        fm.Extrude(New Point3D(0, +0.5, 0), New Point3D(100, +0.5, 1), 0.1, New Vector3D(0, +10, 0), 2)

        fm.RefineElements(2, 2, 2)

        'fm.ElevateElementOrder();

        fm.FixAll(Plane.YZ, 0.1)

        fm.SetPressure(New Point3D(90, -10.5, 1), New Point3D(100, -4, 1), 0.1, 100)

        viewportLayout.Entities.Add(fm)

        Dim s1 As New Solver(fm)

        viewportLayout.StartWork(s1)
    End Sub

    Public Shared Sub TaperedBeam(viewportLayout As ViewportLayout)
        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        fm = New RectangularFemMesh(0, -20, 50, 20)

        fm.Vertices(1).Y = -8

        fm.RefineElements(14, 4)

        fm.Rotate(Math.PI / 2, Vector3D.AxisX, Point3D.Origin)

        fm.Extrude(New Vector3D(0, 1, 0))

        fm.Revolve(New Point3D(20, -5, 0), New Point3D(60, 5, 0), 0.1, Math.PI / 2, Vector3D.AxisX, New Point3D(0, -2, 0), _
            4)

        fm.Extrude(New Point3D(20, -2, 0), New Point3D(60, -2, 10), 0.1, New Vector3D(0, -10, 0), 4)

        fm.RefineElements(2, 2, 2)

        'fm.ElevateElementOrder();

        fm.FixAll(New Point3D(0, -2, -7), New Point3D(0, +2, -2), 0.1)

        fm.FixAll(New Point3D(0, -2, -18), New Point3D(0, +2, -13), 0.1)

        fm.SetPressure(New Point3D(36, -20, 3), New Point3D(60, 0, 10), 0.1, 400)

        viewportLayout.Entities.Add(fm)

        Dim solver As New Solver(fm)

        viewportLayout.StartWork(solver)
    End Sub

    Public Shared Sub Revolve(viewportLayout As ViewportLayout)
        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        ' Show the legend
        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).ColorTable = Legend.RedToBlue9
            viewportLayout.GetLegends()(0).IsSlave = True
            viewportLayout.GetLegends()(0).ItemSize = New Size(10, 24)
            viewportLayout.GetLegends()(0).Visible = True
        End If

        Dim l1 As New Line(50, 0, 110, 0)
        Dim l2 As New Line(110, 0, 110, 70)
        Dim a1 As New Arc(100, 70, 0, 10, 0, Math.PI / 2)
        Dim l3 As New Line(100, 80, 0, 80)
        Dim l4 As New Line(50, 80, 50, 0)

        Dim cc1 As New CompositeCurve(l1, l2, a1, l3, l4)

        Dim r1 As New devDept.Eyeshot.Entities.Region(cc1)

        Dim r2 As devDept.Eyeshot.Entities.Region = New CircularRegion(110, 20, 10)

        Dim r3 As devDept.Eyeshot.Entities.Region = devDept.Eyeshot.Entities.Region.Difference(r1, r2)(0)

        Dim r4 As devDept.Eyeshot.Entities.Region = New CircularRegion(110, 50, 10)

        Dim r5 As devDept.Eyeshot.Entities.Region = devDept.Eyeshot.Entities.Region.Difference(r3, r4)(0)

        Dim r6 As devDept.Eyeshot.Entities.Region = New RectangularRegion(60, 10, 20, 60)

        Dim r7 As devDept.Eyeshot.Entities.Region = devDept.Eyeshot.Entities.Region.Difference(r5, r6)(0)

        Dim m As Mesh = r7.Triangulate(5)

        fm = m.ConvertToFemMesh(Material.StructuralSteel, False)

        fm.Rotate(Math.PI / 2, Vector3D.AxisX, Point3D.Origin)

        fm.Revolve(Math.PI / 2, Vector3D.AxisZ, Point3D.Origin, 16)

        'fm.ElevateElementOrder();

        fm.FixAll(Plane.XY, 0.1)

        Dim plane__1 As Plane = Plane.XY
        plane__1.Translate(0, 0, 80)

        fm.SetPressure(plane__1, 0.1, New Vector3D(-100, 0, -100))

        viewportLayout.Entities.Add(fm)

        '  CLIPPING PLANE
        Dim clippingPlane As New Plane(New Point3D(0, 0, 1), New Vector3D(-0.5, -0.1, 0.6))
        viewportLayout.Entities.Add(New PlanarEntity(clippingPlane, 100), 0, System.Drawing.Color.Magenta)
        fm.ClippingPlane = clippingPlane

        Dim solver As New Solver(fm)

        viewportLayout.StartWork(solver)
    End Sub


    Public Shared Sub Bracket(viewportLayout As ViewportLayout)
        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        ' Show the legend
        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).ColorTable = Legend.RedToBlue9
            viewportLayout.GetLegends()(0).IsSlave = True
            viewportLayout.GetLegends()(0).ItemSize = New Size(10, 24)
            viewportLayout.GetLegends()(0).Visible = True
        End If

        fm = New FemMesh()

        Dim l1 As New Line(0, 0, 10, 8, 0, 10)
        Dim l2 As New Line(8, 0, 10, 38, 0, 40)
        Dim l3 As New Line(38, 0, 40, 38, 0, 48)
        Dim l4 As New Line(38, 0, 48, 8, 0, 48)
        Dim l5 As New Line(8, 0, 48, 0, 0, 40)
        Dim l6 As New Line(0, 0, 40, 0, 0, 10)

        Dim lines As New List(Of ICurve)()
        lines.Add(l1)
        lines.Add(l2)
        lines.Add(l3)
        lines.Add(l4)
        lines.Add(l5)
        lines.Add(l6)

        Dim cc As New CompositeCurve(lines)

        Dim hole As New Circle(Plane.ZX, New Point3D(14, 0, 34), 8)

        Dim r1 As New devDept.Eyeshot.Entities.Region(cc, hole)

        r1.Rotate(-Math.PI / 2, Vector3D.AxisX)

        Dim m as Mesh = r1.Triangulate(2)

        Dim fm1 as FemMesh = m.ConvertToFemMesh(Material.Aluminium, false)

        fm1.Extrude(Plane.XY, 0.1, New Vector3D(0, 0, 2))

        fm1.Rotate(Math.PI / 2, Vector3D.AxisX)
        
        '----------------------------------------------------------------

        fm.CreateRectangularRegion(New Point3D(0, 2, 0), 2, 16, 1, 8, Material.Aluminium)
        fm.CreateSquarePlateWithCircularHole(16, New Point3D(10, 10, 0), 4, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(18, 2, 0), 8, 16, 4, 8, Material.Aluminium)
        fm.CreateSquarePlateWithCircularHole(16, New Point3D(34, 10, 0), 4, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(42, 2, 0), 2, 16, 1, 8, Material.Aluminium)
        fm.CreateRectangularRegion(Point3D.Origin, 2, 2, 1, 1, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(2, 0, 0), 16, 2, 8, 1, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(18, 0, 0), 8, 2, 4, 1, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(26, 0, 0), 16, 2, 8, 1, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(42, 0, 0), 2, 2, 1, 1, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(0, 18, 0), 2, 10, 1, 5, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(2, 18, 0), 16, 10, 8, 5, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(18, 18, 0), 8, 10, 4, 5, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(26, 18, 0), 16, 10, 8, 5, Material.Aluminium)
        fm.CreateRectangularRegion(New Point3D(42, 18, 0), 2, 10, 1, 5, Material.Aluminium)

        Dim fm2 As FemMesh = DirectCast(fm.Clone(), FemMesh)

        fm.Rotate(Plane.XY, 0.1, -Math.PI / 2, Vector3D.AxisY, Point3D.Origin, False)
        fm.Extrude(Plane.YZ, 0.1, New Vector3D(-4, 0, 0), 2)

        fm2.Translate(Plane.XY, 0.1, 4, 0, 48, False)
        fm2.Extrude(New Point3D(4, 0, 48), New Point3D(48, 28, 48), 0.1, New Vector3D(0, 0, 4), 2)

        fm.AddElementsAndNodesToCurrentMesh(fm2.Vertices, fm2.Elements)
        fm.MergeNearbyNodes()

        Dim plane__1 As Plane = Plane.XY
        plane__1.Translate(-8, 0, 44)
        Dim faces As Tuple(Of Integer, Integer)() = fm.GetFaces(plane__1, 0.1)

        fm.Revolve(faces, Math.PI / 2, Vector3D.AxisY, New Point3D(4, 16, 44), 5)

        fm.Translate(0, -28, 0)

        fm.AddElementsAndNodesToCurrentMesh(fm1.Vertices, fm1.Elements)

        fm.Mirror(New Point3D(-10, -30, 0), New Point3D(60, 60, 60), 0.1, Plane.ZX, True)

        fm.MergeNearbyNodes()

        '-----------------------------------------------------------------------

        fm.SetPressure(New Point3D(0, -28, 52), New Point3D(48, 0, 52), 0.1, New Vector3D(0, 0, -500))

        fm.FixAll(New Point3D(-2, -18, 10), Vector3D.AxisX, Vector3D.AxisY, 4, New Interval(0, 2 * Math.PI), New Interval(-2, 2), _
            0.1)
        fm.FixAll(New Point3D(-2, -18, 34), Vector3D.AxisX, Vector3D.AxisY, 4, New Interval(0, 2 * Math.PI), New Interval(-2, 2), _
            0.1)
        fm.FixAll(New Point3D(-2, 18, 34), Vector3D.AxisX, Vector3D.AxisY, 4, New Interval(0, 2 * Math.PI), New Interval(-2, 2), _
            0.1)

        viewportLayout.Entities.Add(fm)

        Dim solver As New Solver(fm)

        viewportLayout.StartWork(solver)
    End Sub

    Public Shared Sub HoledTBeam(viewportLayout As ViewportLayout)
        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        ' Show the legend
        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).ColorTable = Legend.RedToBlue9
            viewportLayout.GetLegends()(0).IsSlave = True
            viewportLayout.GetLegends()(0).ItemSize = New Size(10, 24)
            viewportLayout.GetLegends()(0).Visible = True
        End If

        fm = New FemMesh()

        fm.CreateSquarePlateWithCircularHole(16, New Point3D(8, 8, 0), 5, Material.Aluminium)

        fm.RefineElements(2, 2)

        fm.Extrude(Plane.XY, 0.1, New Vector3D(0, 0, 2), 2)
        Dim plane__1 As Plane = Plane.ZX
        fm.Extrude(plane__1, 0.1, New Vector3D(0, -1, 0), 2)
        plane__1.Translate(0, 16, 0)
        fm.Extrude(plane__1, 0.1, New Vector3D(0, 1, 0), 2)
        fm.Extrude(New Point3D(0, -1, 0), New Point3D(24, 0, 0), 0.1, New Vector3D(0, 0, -4), 4)
        fm.Extrude(New Point3D(0, 16, 0), New Point3D(24, 21, 0), 0.1, New Vector3D(0, 0, -4), 4)
        fm.Extrude(New Point3D(0, 16, 2), New Point3D(24, 21, 2), 0.1, New Vector3D(0, 0, 4), 4)
        fm.Extrude(New Point3D(0, -1, 2), New Point3D(24, 0, 2), 0.1, New Vector3D(0, 0, 4), 4)

        fm.Translate(16, 0, 0, True)
        fm.Translate(32, 0, 0, True)

        fm.MergeNearbyNodes()

        'fm.ElevateElementOrder();

        fm.Rotate(Math.PI / 2, Vector3D.AxisX)

        fm.FixAllNodes(New Point3D(1, -8, -1), New Point3D(1, 6, -1), 0.1)
        fm.FixNodes(New Point3D(63, -8, -1), New Point3D(63, 6, -1), 0.1, False, False, True)
        fm.SetPressure(New Point3D(36, -1, 17), New Point3D(43, 6, 17), 0.1, New Vector3D(0, 0, -200))

        viewportLayout.Entities.Add(fm)

        Dim solver As New Solver(fm)

        viewportLayout.StartWork(solver)

    End Sub

    Public Shared Sub HookBeam(viewportLayout As ViewportLayout)
        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        ' Show the legend
        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).ColorTable = Legend.RedToBlue9
            viewportLayout.GetLegends()(0).IsSlave = True
            viewportLayout.GetLegends()(0).ItemSize = New Size(10, 24)
            viewportLayout.GetLegends()(0).Visible = True
        End If

        fm = New FemMesh()

        fm.CreateCircularPlate(Point3D.Origin, 20, 3, Material.Aluminium)

        fm.Revolve(-Math.PI, Vector3D.AxisY, New Point3D(80, 0, 0), 40)

        fm.Revolve(New Point3D(140, -20, 0), New Point3D(180, 20, 0), 0.1, -Math.PI / 6, Vector3D.AxisY, New Point3D(40, 0, 0), _
            10)

        Dim plane__1 As Plane = Plane.XY
        plane__1.Rotate(-Math.PI / 6, Vector3D.AxisY)
        plane__1.Translate(126, -20, 50)

        fm.Extrude(plane__1, New Interval(0, 100), New Interval(0, 100), 0.8, plane__1.AxisZ * 100, 15)

        viewportLayout.Entities.Add(fm)

        plane__1.Translate(plane__1.AxisZ * 100)

        ' fm.ElevateElementOrder();

        fm.FixAll(plane__1, 0.5)
        DirectCast(fm.Vertices(1440), Node).SetForce(0, 0, -1000)

        Dim solver As New Solver(fm)

        viewportLayout.StartWork(solver)

    End Sub

    Public Shared Sub Beam(viewportLayout As ViewportLayout)

        Dim hollowSquare As New MaterialBeamHollowRect(System.Drawing.Color.SlateGray, 200000, 0.3, 0, 7850, 0.000012, 200, 200, 4)

        Dim nodes As New List(Of Point3D)()
        Dim elements As New List(Of Element)()

        AddBeam(New NodeBeam(100, -100, 0), New NodeBeam(100, -100, 3000), hollowSquare, Vector3D.AxisX, nodes, elements)
        AddBeam(New NodeBeam(3100, -100, 0), New NodeBeam(3100, -100, 3000), hollowSquare, Vector3D.AxisX, nodes, elements)
        AddBeam(New NodeBeam(6100, -100, 0), New NodeBeam(6100, -100, 3000), hollowSquare, Vector3D.AxisX, nodes, elements)
        AddBeam(New NodeBeam(9100, -100, 0), New NodeBeam(9100, -100, 3000), hollowSquare, Vector3D.AxisX, nodes, elements)

        AddBeam(New NodeBeam(100, -4100, 0), New NodeBeam(100, -4100, 3000), hollowSquare, Vector3D.AxisX, nodes, elements)
        AddBeam(New NodeBeam(5100, -4100, 0), New NodeBeam(5100, -4100, 3000), hollowSquare, Vector3D.AxisX, nodes, elements)
        AddBeam(New NodeBeam(7100, -4100, 0), New NodeBeam(7100, -4100, 3000), hollowSquare, Vector3D.AxisX, nodes, elements)
        AddBeam(New NodeBeam(9100, -4100, 0), New NodeBeam(9100, -4100, 3000), hollowSquare, Vector3D.AxisX, nodes, elements)

        Dim Isec As New MaterialBeamI(System.Drawing.Color.DeepPink, 200000, 0.3, 0, 7850, 0.000012, 240, 400, 10, 20)

        For i As Integer = 0 To 8
            AddBeam(New Point3D(100 + i * 1000, -100, 3000), New Point3D(100 + (i + 1) * 1000, -100, 3000), Isec, Vector3D.AxisZ, nodes, elements)
            AddBeam(New Point3D(100 + i * 1000, -4100, 3000), New Point3D(100 + (i + 1) * 1000, -4100, 3000), Isec, Vector3D.AxisZ, nodes, elements)
        Next

        Dim hollowRect As New MaterialBeamHollowRect(System.Drawing.Color.Gold, 200000, 0.3, 0, 7850, 0.000012, _
            180, 380, 4)

        For i As Integer = 0 To 9
            AddBeam(New Point3D(100 + i * 1000, -100, 3000), New Point3D(100 + i * 1000, -4100, 3000), hollowRect, Vector3D.AxisZ, nodes, elements, _
                -10)
        Next

        fm = New FemMesh(nodes, elements)

        fm.MergeNearbyNodes(1)

        viewportLayout.Entities.Add(fm)

        For i As Integer = 0 To fm.Vertices.Length - 1
            If fm.Vertices(i).Z = 0 Then
                DirectCast(fm.Vertices(i), NodeBeam).SetRestraint(True, True, True)
                DirectCast(fm.Vertices(i), NodeBeam).SetRotationRestraint(True, True, True)
            End If
        Next

        Dim solver As New DirectSolver(fm)

        viewportLayout.StartWork(solver)

        Dim legend As Legend

        If viewportLayout.GetLegends().Count > 0 Then
            legend = viewportLayout.GetLegends(0)
        Else
            legend = New Legend(legend.RedToBlue9)
        End If

        legend.Subtitle = "(mm)"
        legend.IsSlave = True
        legend.FormatString = "{0:+0.######;-0.######;0}"

        viewportLayout.ZoomFit()

        viewportLayout.Rendered.PlanarReflections = False

    End Sub

    Private Shared Sub AddBeam(start As Point3D, [end] As Point3D, matBeam As MaterialBeam, w As Vector3D, nodes As List(Of Point3D), elements As List(Of Element), _
        Optional load As Double = 0)

        Dim n1 As New NodeBeam(start.X, start.Y, start.Z)
        Dim index1 As Integer = nodes.Count
        nodes.Add(n1)
        Dim n2 As New NodeBeam([end].X, [end].Y, [end].Z)
        Dim index2 As Integer = nodes.Count
        nodes.Add(n2)

        Dim beam As New Beam(index1, index2, matBeam)

        beam.SetLocalCoordinates(nodes.ToArray(), w)

        elements.Add(beam)

        If load <> 0 Then
            beam.SetDistributedLoad(load, nodes.ToArray())
        End If
    End Sub

    Public Shared Sub Beam2D(viewportLayout As ViewportLayout)
        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        ' Show the legend
        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).ColorTable = Legend.RedToBlue9
            viewportLayout.GetLegends()(0).IsSlave = True
            viewportLayout.GetLegends()(0).ItemSize = New Size(10, 24)
            viewportLayout.GetLegends()(0).Visible = True
            viewportLayout.GetLegends()(0).FormatString = "{0:0.000000}"
        End If

        ' Define Material
        Dim steel As Material = Material.StructuralSteel
        steel.Young = 210000000000.0
        steel.Poisson = 0.25

        Dim matRect As New MaterialBeamI(steel, 0.2, 0.4, 0.01, 0.04)


        '#Region "Mesh definition"

        ' Initialize FemMesh
        fm = New FemMesh(5, 6)

        ' Define vertices
        fm.Vertices(0) = New NodeBeam(0, 0, 0)
        fm.Vertices(1) = New NodeBeam(5, 0, 0)
        fm.Vertices(2) = New NodeBeam(10, 0, 0)
        fm.Vertices(3) = New NodeBeam(7.5, 5, 0)
        fm.Vertices(4) = New NodeBeam(2.5, 5, 0)

        fm.Elements(0) = New Beam2D(0, 1, matRect)
        fm.Elements(1) = New Beam2D(1, 2, matRect)
        fm.Elements(2) = New Beam2D(2, 3, matRect)
        fm.Elements(3) = New Beam2D(3, 1, matRect)
        fm.Elements(4) = New Beam2D(1, 4, matRect)
        fm.Elements(5) = New Beam2D(4, 0, matRect)

        '#End Region


        DirectCast(fm.Vertices(0), Node).SetRestraint(True, True, True, 0, 0, 0)
        DirectCast(fm.Vertices(2), Node).SetRestraint(True, True, True, 0, 0, 0)

        ' Add pressure load
        ' On elements 0 & 1 distributed load 
        DirectCast(fm.Elements(0), Beam2D).SetDistributedLoad(New Vector3D(0, -500, 0), fm.Vertices)
        DirectCast(fm.Elements(1), Beam2D).SetDistributedLoad(New Vector3D(0, -500, 0), fm.Vertices)

        ' Add FemMesh to entities collection
        viewportLayout.Entities.Add(fm)

        viewportLayout.SetView(viewType.Trimetric)
        viewportLayout.ZoomFit()

        Dim solver As New DirectSolver(fm)

        viewportLayout.StartWork(solver)

    End Sub

    Public Shared Sub Mesher(viewportLayout As ViewportLayout)
        MainWindow.SetDisplayMode(viewportLayout, displayType.Shaded)

        viewportLayout.GetLegends()(0).ColorTable = Legend.RedToBlue9
        viewportLayout.GetLegends()(0).IsSlave = True
        viewportLayout.GetLegends()(0).ItemSize = New Size(10, 24)

        Dim l1 As New Line(0, 0, 100, 0)
        Dim l2 As New Line(100, 0, 100, 30)
        Dim l3 As New Line(100, 30, 80, 30)
        Dim a1 As New Arc(80, 50, 0, 20, Utility.DegToRad(180), Utility.DegToRad(270))
        Dim l4 As New Line(60, 50, 60, 80)
        Dim l5 As New Line(60, 80, 30, 80)
        Dim l6 As New Line(30, 80, 30, 31)
        Dim a2 As New Arc(29, 31, 0, 1, Utility.DegToRad(270), Utility.DegToRad(360))
        Dim l7 As New Line(29, 30, 26, 30)
        Dim a3 As New Arc(26, 31, 0, 1, Utility.DegToRad(180), Utility.DegToRad(270))
        Dim l8 As New Line(25, 31, 25, 80)

        Dim l9 As New Line(25, 80, 15, 80)
        Dim l10 As New Line(15, 80, 15, 31)
        Dim a4 As New Arc(14, 31, 0, 1, Utility.DegToRad(270), Utility.DegToRad(360))
        Dim l11 As New Line(14, 30, 11, 30)
        Dim a5 As New Arc(11, 31, 0, 1, Utility.DegToRad(180), Utility.DegToRad(270))
        Dim l12 As New Line(10, 31, 10, 80)
        Dim l13 As New Line(10, 80, 0, 80)
        Dim l14 As New Line(0, 80, 0, 0)

        Dim c1 As New Circle(20, 15, 0, 5)
        Dim c2 As New Circle(35, 15, 0, 5)
        Dim c3 As New Circle(50, 15, 0, 5)
        Dim c4 As New Circle(65, 15, 0, 5)

        Dim reg As New Region(New CompositeCurve(l1, l2, l3, l4, l5, l6, l7, l8, l9, l10, l11, l12, _
	            l13, l14, a1, a2, a3, a4, a5), c1, c2, c3, c4)

        Dim m As Mesh = UtilityEx.Triangulate(reg, 3)
        
        Dim copper As Material = Material.Copper
        copper.ElementThickness = 6
        copper.ElementType = elementType.PlaneStress
        
        fm = m.ConvertToFemMesh(copper, True)
        
        viewportLayout.Entities.Add(fm)
        
        '#Region "Boundary conditions"
        
        fm.FixAll(New Point3D(0, 0), New Point3D(100, 0), 0.1)
        
        fm.SetPressure(New Point3D(45, 80), New Point3D(55, 80), 0.1, 50)
        
        '#End Region
        
        Dim solver As New Solver(fm)

        viewportLayout.StartWork(solver)

    End Sub

    Public Shared Sub PostProcessing(viewportLayout As ViewportLayout)
        If viewportLayout.GetLegends().Count > 0 Then
            viewportLayout.GetLegends()(0).Visible = True

            viewportLayout.GetLegends()(0).IsSlave = True

            ' Changes the Legend color box size
            viewportLayout.GetLegends()(0).ItemSize = New Size(9, 18)

            ' Changes the Legend color table
            viewportLayout.GetLegends()(0).ColorTable = New ObservableCollection(Of Brush)() From { _
                Brushes.DarkRed, _
                Brushes.DarkOrange, _
                Brushes.DarkGoldenrod, _
                Brushes.White, _
                Brushes.Gainsboro, _
                Brushes.YellowGreen, _
                Brushes.NavajoWhite, _
                Brushes.DarkOliveGreen, _
                Brushes.Blue _
            }
        End If

        fm = New FemMesh(13, 2)

        ' Mesh nodes positions
        fm.Vertices(0) = New Node(0, 0)
        fm.Vertices(1) = New Node(5, 0)
        fm.Vertices(2) = New Node(10, 0)
        fm.Vertices(3) = New Node(15, 0)
        fm.Vertices(4) = New Node(20, 0)
        fm.Vertices(5) = New Node(0, 5)
        fm.Vertices(6) = New Node(10, 5)
        fm.Vertices(7) = New Node(20, 5)
        fm.Vertices(8) = New Node(0, 10)
        fm.Vertices(9) = New Node(5, 10)
        fm.Vertices(10) = New Node(10, 10)
        fm.Vertices(11) = New Node(15, 10)
        fm.Vertices(12) = New Node(20, 10)

        Dim mat As New Material()

        ' Mesh elements             
        fm.Elements(0) = New Quad8(0, 1, 2, 6, 10, 9, _
            8, 5, mat)
        fm.Elements(1) = New Quad8(2, 3, 4, 7, 12, 11, _
            10, 6, mat)

        viewportLayout.ShowVertices = True

        ' Sets some nodal stress values
        DirectCast(fm.Vertices(6), Node).Stress = New Double() {10, 100, 0, 100, 0, 0}
        DirectCast(fm.Vertices(10), Node).Stress = New Double() {10, 100, 0, 100, 0, 0}

        ' Sets some nodal displacements
        DirectCast(fm.Vertices(11), Node).Unknowns(1) = 1
        DirectCast(fm.Vertices(12), Node).Unknowns(1) = 2

        ' Updates VonMises and Principals stresses (derived by the 6 Stress values above)
        For Each nd As Node In fm.Vertices

            nd.UpdateVonMisesAndPrincipals()
        Next

        ' Sets the desired plot type
        fm.PlotMode = FemMesh.plotType.P1
        fm.NodalAverages = True

        viewportLayout.Entities.Add(fm, 0, System.Drawing.Color.Gray)

        fm.ComputePlot(viewportLayout, viewportLayout.GetLegends()(0), True)

        ' Replace standard legend title
        viewportLayout.GetLegends()(0).Title = "My legend title"

    End Sub

End Class
