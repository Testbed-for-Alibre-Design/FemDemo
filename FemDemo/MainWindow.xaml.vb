Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Linq
Imports System.Security.AccessControl
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports devDept.Eyeshot
Imports devDept.Eyeshot.Fem
Imports devDept.Eyeshot.Labels
Imports devDept.Eyeshot.Triangulation
Imports devDept.Graphics
Imports devDept.Eyeshot.Entities
Imports devDept.Geometry
Imports Microsoft.Win32
Imports Cursors = System.Windows.Input.Cursors
Imports MouseButton = System.Windows.Input.MouseButton
Imports devDept.Eyeshot.Translators
Public Enum FemElementsType
    None
    Beam2D
    Beam3D
    Element2D
    Element3D
End Enum
''' <summary>
''' Interaction logic for MainWindow.xaml
''' </summary>
Partial Public Class MainWindow    
    Public Sub New()
        InitializeComponent()
         viewportLayout1.Unlock("ULTWPF-0216-ASHFJ-M8XER-MHNPA") ' For more details see 'Product Activation' topic in the documentation.      
        AddHandler viewportLayout1.Viewports(0).LabelSelectionChanged, AddressOf viewportZero_LabelSelectionChanged
        SetPropertyGridObject(Nothing)
    End Sub
    Private _isContentRendered As Boolean
    Protected Overrides Sub OnContentRendered(e As EventArgs)       
        tabControl1.SelectedIndex = 7
        FillPlotTypeComboBox()
        contourPlotCheckBox.IsChecked = True
        nodalAveragesCheckBox.IsChecked = True
        rendererVersionStatusLabel.Text = viewportLayout1.RendererVersion.ToString()
        MyBase.OnContentRendered(e)
        viewportLayout1.Invalidate()
        _isContentRendered = True
    End Sub
    Friend Shared Function GetFemElementsType(femMesh As FemMesh) As FemElementsType
        If femMesh Is Nothing Then
            Return FemElementsType.None
        End If
        If TypeOf femMesh.Elements(0) Is Beam2D Then
            Return FemElementsType.Beam2D
        End If
        If TypeOf femMesh.Elements(0) Is Beam Then
            Return FemElementsType.Beam3D
        End If
        If TypeOf femMesh.Elements(0) Is Element2D Then
            Return FemElementsType.Element2D
        End If
        If TypeOf femMesh.Elements(0) Is Element3D Then
            Return FemElementsType.Element3D
        End If
        Return FemElementsType.None
    End Function
    Private Sub FillPlotTypeComboBox()
        plotTypeComboBox.Items.Clear()
        plotTypeComboBox.Items.Add("Mesh")
        Dim elementType = GetFemElementsType(Draw.fm)
        plotTypeComboBox.Items.Add("Ux")
        plotTypeComboBox.Items.Add("Uy")
        If elementType = FemElementsType.Beam3D OrElse elementType = FemElementsType.Element3D Then
            plotTypeComboBox.Items.Add("Uz")
        End If
        plotTypeComboBox.Items.Add("U")
        If elementType = FemElementsType.Beam3D Then
            plotTypeComboBox.Items.Add("Rx")
            plotTypeComboBox.Items.Add("Ry")
        End If
        If elementType = FemElementsType.Beam2D OrElse elementType = FemElementsType.Beam3D Then
            plotTypeComboBox.Items.Add("Rz")
            plotTypeComboBox.Items.Add("Axial Force")
            plotTypeComboBox.Items.Add("Shear Force V")
        End If
        If elementType = FemElementsType.Beam3D Then
            plotTypeComboBox.Items.Add("Shear Force W")
            plotTypeComboBox.Items.Add("Torsion Moment")
            plotTypeComboBox.Items.Add("Bending Moment V")
        End If
        If elementType = FemElementsType.Beam2D OrElse elementType = FemElementsType.Beam3D Then
            plotTypeComboBox.Items.Add("Bending Moment W")
        End If
        If elementType = FemElementsType.Beam3D Then
            plotTypeComboBox.Items.Add("Twist Angle")
        End If
        If elementType = FemElementsType.Element2D OrElse elementType = FemElementsType.Element3D Then
            plotTypeComboBox.Items.Add("X stress")
            plotTypeComboBox.Items.Add("Y stress")
            If elementType = FemElementsType.Element3D Then
                plotTypeComboBox.Items.Add("Z stress")
            End If
            plotTypeComboBox.Items.Add("XY shear")
            If elementType = FemElementsType.Element3D Then
                plotTypeComboBox.Items.Add("YZ shear")
                plotTypeComboBox.Items.Add("XZ shear")
            End If
            plotTypeComboBox.Items.Add("Maximum Principal")
            plotTypeComboBox.Items.Add("Intermediate Principal")
            If elementType = FemElementsType.Element3D Then
                plotTypeComboBox.Items.Add("Minimum Principal")
            End If
            plotTypeComboBox.Items.Add("Von Mises")
            plotTypeComboBox.Items.Add("Tresca")
        End If
        'plotTypeComboBox.SelectedIndex = plotTypeComboBox.Items.Count - 2;
        ' we don't want to replace the custom legend in the PostProcessing example
        If tabControl1.SelectedItem.Equals(postprocessingTabPage) Then
            Return
        End If
        ' here we set the PlotType to displacement magnitude, a quantity shared by all elements
        If elementType = FemElementsType.Beam3D OrElse elementType = FemElementsType.Element3D Then
            plotTypeComboBox.SelectedIndex = 4
        Else
            plotTypeComboBox.SelectedIndex = 3
        End If
    End Sub
    Private Sub tabControl1_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If viewportLayout1.renderContext Is Nothing Then
            Return
        End If
        ' every time the selected tab changes ...
        viewportLayout1.ActionMode = actionType.None ' reset all actions
        viewportLayout1.Focus()
        perspectiveButton.IsChecked = True ' set default projection to perspective
        selectionComboBox.SelectedIndex = 5 ' set default selection to VisibleByPick
        selectButton.IsChecked = False ' disable selection mode
        viewportLayout1.ShowVertices = False
        viewportLayout1.StopAnimation() ' stop any animation
        viewportLayout1.Clear() ' clear viewportLayout (entities, blocks, layers, materials, etc.)
        SetPropertyGridObject(Nothing) ' clear propertyGrid contents
        If viewportLayout1.GetLegends().Count > 0 Then
            viewportLayout1.GetLegends()(0).Visible = False
        End If
        viewportLayout1.GetGrid().Visible = True
        viewportLayout1.GetGrid().[Step] = 10
        viewportLayout1.HiddenLines.Lighting = False
        viewportLayout1.HiddenLines.ColorMethod = hiddenLinesColorMethodType.SingleColor
        viewportLayout1.HiddenLines.DashedHiddenLines = False
        viewportLayout1.AutoHideLabels = True
        viewportLayout1.DisplayMode = displayType.Rendered
        Select Case DirectCast(tabControl1.SelectedItem, TabItem).Header.ToString()
            Case "Hex20"
                Draw.Hex20(viewportLayout1)
                Exit Select
            Case "Quad8"
                Draw.Quad8(viewportLayout1)
                Exit Select
            Case "Truss"
                Draw.Truss(viewportLayout1)
                Exit Select
            Case "T-Shaped Beam"
                Draw.TShapedBeam(viewportLayout1)
                Exit Select
            Case "Cantilever Beam"
                Draw.CantileverBeam(viewportLayout1)
                Exit Select
            Case "Tapered Beam"
                Draw.TaperedBeam(viewportLayout1)
                Exit Select
            Case "Revolve"
                Draw.Revolve(viewportLayout1)
                Exit Select
            Case "Bracket"
                Draw.Bracket(viewportLayout1)
                Exit Select
            Case "Hook"
                Draw.HookBeam(viewportLayout1)
                Exit Select
            Case "I-Beam"
                Draw.HoledTBeam(viewportLayout1)
                Exit Select
            Case "Beam"
                Draw.Beam(viewportLayout1)
                Exit Select
            Case "Beam2D"
                Draw.Beam2D(viewportLayout1)
                Exit Select
            Case "Mesher"
                Draw.Mesher(viewportLayout1)
                Exit Select
            Case "Post-Processing"
                Draw.PostProcessing(viewportLayout1)
                FillPlotTypeComboBox()
                Exit Select
        End Select
        If viewportLayout1.IsBusy Then
            tabControl1.IsEnabled = False
            importButton.IsEnabled = False
            plotTypeComboBox.IsEnabled = False
            numericResultsButton.IsEnabled = False
        Else
            plotTypeComboBox.IsEnabled = True
            numericResultsButton.IsEnabled = True
        End If
        If _isContentRendered Then
            ' Sets trimetric view and fits the model in the main viewport
            viewportLayout1.SetView(viewType.Trimetric, True, viewportLayout1.AnimateCamera)
            ' Refresh the viewportLayout
            viewportLayout1.Invalidate()
            UpdateDisplayModeButtons()
        End If
    End Sub
#Region "DisplayMode"
    Private Sub UpdateDisplayModeButtons()
        ' syncs the shading buttons with the current display mode.
        Select Case viewportLayout1.DisplayMode
            Case displayType.Wireframe
                wireframeButton.IsChecked = True
                SetDisplayModeButtonsChecked(wireframeButton)
                Exit Select
            Case displayType.Shaded
                shadedButton.IsChecked = True
                SetDisplayModeButtonsChecked(shadedButton)
                Exit Select
            Case displayType.Rendered
                renderedButton.IsChecked = True
                SetDisplayModeButtonsChecked(renderedButton)
                Exit Select
            Case displayType.Flat
                flatButton.IsChecked = True
                SetDisplayModeButtonsChecked(flatButton)
                Exit Select
            Case displayType.HiddenLines
                hiddenLinesButton.IsChecked = True
                SetDisplayModeButtonsChecked(hiddenLinesButton)
                Exit Select
        End Select
    End Sub
    Private Sub SetDisplayModeButtonsChecked(checkedButton As ToggleButton)
        If Not checkedButton.Equals(wireframeButton) Then
            wireframeButton.IsChecked = Not checkedButton.IsChecked
        End If
        If Not checkedButton.Equals(shadedButton) Then
            shadedButton.IsChecked = Not checkedButton.IsChecked
        End If
        If Not checkedButton.Equals(renderedButton) Then
            renderedButton.IsChecked = Not checkedButton.IsChecked
        End If
        If Not checkedButton.Equals(hiddenLinesButton) Then
            hiddenLinesButton.IsChecked = Not checkedButton.IsChecked
        End If
        If Not checkedButton.Equals(flatButton) Then
            flatButton.IsChecked = Not checkedButton.IsChecked
        End If
    End Sub
    Private Sub wireframeButton_OnClick(sender As Object, e As RoutedEventArgs)
        SetDisplayModeButtonsChecked(DirectCast(sender, ToggleButton))
        SetDisplayMode(viewportLayout1, displayType.Wireframe)
    End Sub
    Private Sub shadedButton_OnClick(sender As Object, e As RoutedEventArgs)
        SetDisplayModeButtonsChecked(DirectCast(sender, ToggleButton))
        SetDisplayMode(viewportLayout1, displayType.Shaded)
    End Sub
    Private Sub renderedButton_OnClick(sender As Object, e As RoutedEventArgs)
        SetDisplayModeButtonsChecked(DirectCast(sender, ToggleButton))
        SetDisplayMode(viewportLayout1, displayType.Rendered)
    End Sub
    Private Sub hiddenLinesButton_OnClick(sender As Object, e As RoutedEventArgs)
        SetDisplayModeButtonsChecked(DirectCast(sender, ToggleButton))
        SetDisplayMode(viewportLayout1, displayType.HiddenLines)
    End Sub
    Private Sub flatButton_OnClick(sender As Object, e As RoutedEventArgs)
        SetDisplayModeButtonsChecked(DirectCast(sender, ToggleButton))
        SetDisplayMode(viewportLayout1, displayType.Flat)
    End Sub
    Private Sub showCurveDirectionButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.ShowCurveDirection = showCurveDirectionButton.IsChecked.Value
        viewportLayout1.Invalidate()
    End Sub
    Public Shared Sub SetDisplayMode(viewportLayout As ViewportLayout, displayType As displayType)
        viewportLayout.DisplayMode = displayType
        SetBackgroundStyleAndColor(viewportLayout)
        viewportLayout.Invalidate()
    End Sub
    Public Shared Sub SetBackgroundStyleAndColor(ByVal viewportLayout As ViewportLayout)
        viewportLayout.GetCoordinateSystemIcon().Lighting = False
        viewportLayout.GetViewCubeIcon().Lighting = False
        Select Case viewportLayout.DisplayMode
            Case displayType.HiddenLines
                viewportLayout.GetBackground().TopColor = RenderContextUtility.ConvertColor(System.Drawing.Color.FromArgb(&HD2, &HD0, &HB9))
                viewportLayout.GetCoordinateSystemIcon().Lighting = True
                viewportLayout.GetViewCubeIcon().Lighting = True
                Exit Select
            Case Else
                viewportLayout.GetBackground().TopColor = RenderContextUtility.ConvertColor(System.Drawing.Color.FromArgb(&HED, &HED, &HED))
                Exit Select
        End Select
        viewportLayout.CompileUserInterfaceElements()
    End Sub
    Private Sub cullingButton_OnClick(sender As Object, e As RoutedEventArgs)
        If cullingButton.IsChecked.Value Then
            viewportLayout1.Backface.ColorMethod = backfaceColorMethodType.Cull
        Else
            viewportLayout1.Backface.ColorMethod = backfaceColorMethodType.EntityColor
        End If
        viewportLayout1.Invalidate()
    End Sub
#End Region
#Region "Projection"
    Private Sub parallelButton_OnClick(sender As Object, e As RoutedEventArgs)
        perspectiveButton.IsChecked = Not parallelButton.IsChecked
        viewportLayout1.Camera.ProjectionMode = projectionType.Orthographic
        viewportLayout1.AdjustNearAndFarPlanes()
        viewportLayout1.Invalidate()
    End Sub
    Private Sub perspectiveButton_OnClick(sender As Object, e As RoutedEventArgs)
        parallelButton.IsChecked = Not perspectiveButton.IsChecked
        viewportLayout1.Camera.ProjectionMode = projectionType.Perspective
        viewportLayout1.AdjustNearAndFarPlanes()
        viewportLayout1.Invalidate()
    End Sub
#End Region
#Region "Zoom/Pan/Rotate"
    Private Sub zoomButton_OnClick(sender As Object, e As EventArgs)
        viewportLayout1.ActionMode = actionType.None
        If zoomButton.IsChecked Then
            viewportLayout1.ActionMode = actionType.Zoom
        End If
        panButton.IsChecked = False
        rotateButton.IsChecked = False
        zoomWindowButton.IsChecked = False
        selectButton.IsChecked = False
    End Sub
    Private Sub panButton_OnClick(sender As Object, e As EventArgs)
        viewportLayout1.ActionMode = actionType.None
        If panButton.IsChecked Then
            viewportLayout1.ActionMode = actionType.Pan
        End If
        zoomButton.IsChecked = False
        rotateButton.IsChecked = False
        zoomWindowButton.IsChecked = False
        selectButton.IsChecked = False
    End Sub
    Private Sub rotateButton_OnClick(sender As Object, e As EventArgs)
        viewportLayout1.ActionMode = actionType.None
        If rotateButton.IsChecked Then
            viewportLayout1.ActionMode = actionType.Rotate
        End If
        zoomButton.IsChecked = False
        panButton.IsChecked = False
        zoomWindowButton.IsChecked = False
        selectButton.IsChecked = False
    End Sub
    Private Sub zoomFitButton_OnClick(sender As Object, e As EventArgs)
        viewportLayout1.ZoomFit()
        viewportLayout1.Invalidate()
    End Sub
    Private Sub zoomWindowButton_OnClick(sender As Object, e As EventArgs)
        viewportLayout1.ActionMode = actionType.None
        If zoomWindowButton.IsChecked Then
            viewportLayout1.ActionMode = actionType.ZoomWindow
        End If
        zoomButton.IsChecked = False
        panButton.IsChecked = False
        rotateButton.IsChecked = False
        selectButton.IsChecked = False
    End Sub
    Private RotateToFace As Boolean = False
    Private Sub rotateToFaceButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.ActionMode = actionType.None
        RotateToFace = False
        If rotateToFaceButton.IsChecked IsNot Nothing AndAlso rotateToFaceButton.IsChecked.Value Then
            RotateToFace = True
            viewportLayout1.Cursor = Cursors.Hand
        Else
            RotateToFace = False
            viewportLayout1.Cursor = Nothing
        End If
    End Sub
#End Region
#Region "Zoom"
    Private Sub zoomSelectionButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.ZoomFit(True)
        viewportLayout1.Invalidate()
    End Sub
#End Region
#Region "View"
    Private Sub isoViewButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.SetView(viewType.Isometric, True, viewportLayout1.AnimateCamera)
        viewportLayout1.Invalidate()
    End Sub
    Private Sub frontViewButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.SetView(viewType.Front, True, viewportLayout1.AnimateCamera)
        viewportLayout1.Invalidate()
    End Sub
    Private Sub sideViewButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.SetView(viewType.Right, True, viewportLayout1.AnimateCamera)
        viewportLayout1.Invalidate()
    End Sub
    Private Sub topViewButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.SetView(viewType.Top, True, viewportLayout1.AnimateCamera)
        viewportLayout1.Invalidate()
    End Sub
    Private Sub prevViewButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.PreviousView()
        viewportLayout1.Invalidate()
    End Sub
    Private Sub nextViewButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.NextView()
        viewportLayout1.Invalidate()
    End Sub
    Private Sub animateCameraCheckBox_OnClick(sender As Object, e As RoutedEventArgs)
        If animateCameraCheckBox.IsChecked IsNot Nothing Then
            viewportLayout1.AnimateCamera = animateCameraCheckBox.IsChecked.Value
        End If
    End Sub
#End Region
#Region "Hide/Show"
    Private Sub showOriginButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.GetOriginSymbol().Visible = showOriginButton.IsChecked.Value
        viewportLayout1.Invalidate()
    End Sub
    Private Sub showExtentsButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.BoundingBox.Visible = showExtentsButton.IsChecked.Value
        viewportLayout1.Invalidate()
    End Sub
    Private Sub showVerticesButton_OnClick(sender As Object, e As RoutedEventArgs)
        If showVerticesButton.IsChecked IsNot Nothing Then
            viewportLayout1.ShowVertices = showVerticesButton.IsChecked.Value
        End If
        viewportLayout1.Invalidate()
    End Sub
    Private Sub showGridButton_OnClick(sender As Object, e As RoutedEventArgs)
        If showGridButton.IsChecked IsNot Nothing Then
            viewportLayout1.GetGrid().Visible = showGridButton.IsChecked.Value
        End If
        viewportLayout1.Invalidate()
    End Sub
#End Region
#Region "Selection"
    Private Sub selectionComboBox_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If selectButton Is Nothing Then
            Return
        End If
        groupButton.IsEnabled = True
        If selectButton.IsChecked.HasValue AndAlso selectButton.IsChecked.Value Then
            Selection()
        End If
    End Sub
    Private Sub selectCheckBox_OnClick(sender As Object, e As RoutedEventArgs)
        groupButton.IsEnabled = True
        If selectButton.IsChecked.HasValue AndAlso selectButton.IsChecked.Value Then
            Selection()
        Else
            viewportLayout1.ActionMode = actionType.None
        End If
    End Sub
    Private Sub Selection()
        Select Case selectionComboBox.SelectedIndex
            Case 0
                ' by pick
                viewportLayout1.ActionMode = actionType.SelectByPick
                Exit Select
            Case 1
                ' by box
                viewportLayout1.ActionMode = actionType.SelectByBox
                Exit Select
            Case 2
                ' by poly
                viewportLayout1.ActionMode = actionType.SelectByPolygon
                Exit Select
            Case 3
                ' by box enclosed
                viewportLayout1.ActionMode = actionType.SelectByBoxEnclosed
                Exit Select
            Case 4
                ' by poly enclosed
                viewportLayout1.ActionMode = actionType.SelectByPolygonEnclosed
                Exit Select
            Case 5
                ' visible by pick
                viewportLayout1.ActionMode = actionType.SelectVisibleByPick
                Exit Select
            Case 6
                ' visible by box
                viewportLayout1.ActionMode = actionType.SelectVisibleByBox
                Exit Select
            Case 7
                ' visible by poly
                viewportLayout1.ActionMode = actionType.SelectVisibleByPolygon
                Exit Select
            Case 8
                ' visible by pick label
                viewportLayout1.ActionMode = actionType.SelectVisibleByPickLabel
                groupButton.IsEnabled = False
                Exit Select
            Case Else
                viewportLayout1.ActionMode = actionType.None
                Exit Select
        End Select
    End Sub
    Private Sub clearSelectionButton_OnClick(sender As Object, e As RoutedEventArgs)
        If viewportLayout1.ActionMode = actionType.SelectVisibleByPickLabel Then
            viewportLayout1.Viewports(0).Labels.ClearSelection()
        Else
            viewportLayout1.Entities.ClearSelection()
        End If
        viewportLayout1.Invalidate()
    End Sub
    Private Sub invertSelectionButton_OnClick(sender As Object, e As RoutedEventArgs)
        If viewportLayout1.ActionMode = actionType.SelectVisibleByPickLabel Then
            viewportLayout1.Viewports(0).Labels.InvertSelection()
        Else
            viewportLayout1.Entities.InvertSelection()
        End If
        viewportLayout1.Invalidate()
    End Sub
    Private Sub groupButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.GroupSelection()
    End Sub
#End Region
#Region "Editing"
    Private Sub duplicateButton_OnClick(sender As Object, e As RoutedEventArgs)
        ' counts selected entities
        Dim count As Integer = 0
        For Each ent As Entity In viewportLayout1.Entities
            If ent.Selected Then
                count += 1
            End If
        Next
        ' fills the duplicates array
        Dim duplicates As Entity() = New Entity(count - 1) {}
        count = 0
        For Each ent As Entity In viewportLayout1.Entities
            If ent.Selected Then
                duplicates(count) = DirectCast(ent.Clone(), Entity)
                ent.Selected = False
                count += 1
            End If
        Next
        For Each dup As Entity In duplicates
            dup.Translate(50, 100, 50)
            viewportLayout1.Entities.Add(dup)
        Next
        viewportLayout1.Invalidate()
    End Sub
    Private Sub deleteButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.Entities.DeleteSelected()
        viewportLayout1.Invalidate()
    End Sub
    Private Sub explodeButton_OnClick(sender As Object, e As RoutedEventArgs)
        For i As Integer = viewportLayout1.Entities.Count - 1 To 0 Step -1
            Dim en As Entity = viewportLayout1.Entities(i)
            If en.Selected Then
                If TypeOf en Is BlockReference Then
                    viewportLayout1.Entities.RemoveAt(i)
                    Dim br As BlockReference = DirectCast(en, BlockReference)
                    Dim entList As Entity() = viewportLayout1.Entities.Explode(br)
                    viewportLayout1.Entities.AddRange(entList)
                ElseIf TypeOf en Is CompositeCurve Then
                    viewportLayout1.Entities.RemoveAt(i)
                    Dim cc As CompositeCurve = DirectCast(en, CompositeCurve)
                    viewportLayout1.Entities.AddRange(cc.Explode())
                ElseIf en.GroupIndex > -1 Then
                    viewportLayout1.Ungroup(en.GroupIndex)
                End If
            End If
        Next
        viewportLayout1.Invalidate()
    End Sub
#End Region
#Region "Inspection"
    Private inspectVertex As Boolean = False
    Private inspectMesh As Boolean = False
    Private Sub pickVertexButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.ActionMode = actionType.None
        inspectVertex = False
        inspectMesh = False
        If pickVertexButton.IsChecked.HasValue AndAlso pickVertexButton.IsChecked.Value Then
            inspectVertex = True
            mainStatusLabel.Content = "Click on the entity to retrieve the 3D coordinates"
        Else
            mainStatusLabel.Content = ""
        End If
        pickFaceButton.IsChecked = False
    End Sub
    Private Sub pickFaceButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.ActionMode = actionType.None
        inspectVertex = False
        inspectMesh = False
        If pickFaceButton.IsChecked.HasValue AndAlso pickFaceButton.IsChecked.Value Then
            inspectMesh = True
            mainStatusLabel.Content = "Click on the face to retrieve the 3D coordinates"
        Else
            mainStatusLabel.Content = ""
        End If
        pickVertexButton.IsChecked = False
    End Sub
    Private Sub ViewportLayout1_MouseDown(sender As Object, e As MouseButtonEventArgs)
        ' Checks that we are not using left mouse button for ZPR
        If viewportLayout1.ActionMode = actionType.None AndAlso e.ChangedButton <> MouseButton.Middle Then
            Dim closest As Point3D = Nothing
            If inspectVertex Then
                If viewportLayout1.FindClosestVertex(RenderContextUtility.ConvertPoint(viewportLayout1.GetMousePosition(e)), 50, closest) <> -1 Then
                    viewportLayout1.Labels.Add(New devDept.Eyeshot.Labels.LeaderAndText(closest, closest.ToString(), New System.Drawing.Font("Tahoma", 8.25F), Draw.Color, New Vector2D(0, 50)))
                End If
            ElseIf inspectMesh Then
                Dim entityIndex As Integer = viewportLayout1.GetEntityUnderMouseCursor(RenderContextUtility.ConvertPoint(viewportLayout1.GetMousePosition(e)))
                If entityIndex <> -1 Then
                    Dim ent As Entity = viewportLayout1.Entities(entityIndex)
                    Dim hitTri As IList(Of HitTriangle) = viewportLayout1.FindClosestTriangle(DirectCast(ent, IFace), RenderContextUtility.ConvertPoint(viewportLayout1.GetMousePosition(e)))
                    If hitTri.Count > 0 Then
                        Dim hitTriangle As HitTriangle = hitTri(0)
                        Dim pt As Point3D = hitTriangle.IntersectionPoint
                        ' adds a label with the point elevation
                        viewportLayout1.Labels.Add(New devDept.Eyeshot.Labels.LeaderAndText(pt, pt.ToString() + " | Element #" + hitTriangle.ShellOrElementIndex.ToString() + " | Face #" + hitTriangle.FaceIndex.ToString(), New System.Drawing.Font("Tahoma", 8.25F), Draw.Color, New Vector2D(0, 50)))
                    End If
                End If
            End If
            If RotateToFace Then
                ' rotates the view perpendicular to the plane under the mouse cursor
                viewportLayout1.RotateCamera(RenderContextUtility.ConvertPoint(viewportLayout1.GetMousePosition(e)))
            End If
            viewportLayout1.Invalidate()
        End If
        If RotateToFace Then
            rotateToFaceButton.IsChecked = False
        End If
    End Sub
    Private Sub dumpButton_OnClick(sender As Object, e As RoutedEventArgs)
        For i As Integer = 0 To viewportLayout1.Entities.Count - 1
            If viewportLayout1.Entities(i).Selected Then
                Dim details As String = "Entity ID = " & i & System.Environment.NewLine + "----------------------" & System.Environment.NewLine & viewportLayout1.Entities(i).Dump()
                Dim rf As New DetailsWindow()
                rf.Title = "Dump"
                rf.contentTextBox.Text = details
                rf.Show()
                Exit For
            End If
        Next
    End Sub
    Private Sub areaButton_OnClick(sender As Object, e As RoutedEventArgs)
        Dim ap As New AreaProperties()
        Dim count As Integer = 0
        For i As Integer = 0 To viewportLayout1.Entities.Count - 1
            Dim ent As Entity = viewportLayout1.Entities(i)
            If ent.Selected Then
                If TypeOf ent Is IFace Then
                    Dim itfFace As IFace = DirectCast(ent, IFace)
                    Dim meshes As Mesh() = itfFace.GetPolygonMeshes()
                    For Each mesh As Mesh In meshes
                        ap.Add(mesh.Vertices, mesh.Triangles)
                    Next
                    count += 1
                Else
                    Dim itfCurve As ICurve = DirectCast(ent, ICurve)
                    If itfCurve.IsClosed Then
                        ap.Add(ent.Vertices)
                    End If
                    count += 1
                End If
            End If
        Next
        Dim text As New StringBuilder()
        text.AppendLine(count.ToString() + " entity(ies) selected")
        text.AppendLine("---------------------")
        If ap.Centroid IsNot Nothing Then
            Dim x As Double, y As Double, z As Double
            Dim xx As Double, yy As Double, zz As Double, xy As Double, zx As Double, yz As Double
            Dim world As MomentOfInertia, centroid As MomentOfInertia
            ap.GetResults(ap.Area, ap.Centroid, x, y, z, xx,
                yy, zz, xy, zx, yz, world,
                centroid)
            text.AppendLine("Cumulative area: " + ap.Area.ToString() + " square " + viewportLayout1.Units.ToString().ToLower())
            text.AppendLine("Cumulative centroid: " + ap.Centroid.ToString())
            text.AppendLine("Cumulative area moments:")
            text.AppendLine(" First moments")
            text.AppendLine("  x: " + x.ToString("g6"))
            text.AppendLine("  y: " + y.ToString("g6"))
            text.AppendLine("  z: " + z.ToString("g6"))
            text.AppendLine(" Second moments")
            text.AppendLine("  xx: " + xx.ToString("g6"))
            text.AppendLine("  yy: " + yy.ToString("g6"))
            text.AppendLine("  zz: " + zz.ToString("g6"))
            text.AppendLine(" Product moments")
            text.AppendLine("  xy: " + xx.ToString("g6"))
            text.AppendLine("  yz: " + yy.ToString("g6"))
            text.AppendLine("  zx: " + zz.ToString("g6"))
            text.AppendLine(" Area Moments of Inertia about World Coordinate Axes")
            text.AppendLine("  Ix: " + world.Ix.ToString("g6"))
            text.AppendLine("  Iy: " + world.Iy.ToString("g6"))
            text.AppendLine("  Iz: " + world.Iz.ToString("g6"))
            text.AppendLine(" Area Radii of Gyration about World Coordinate Axes")
            text.AppendLine("  Rx: " + world.Rx.ToString("g6"))
            text.AppendLine("  Ry: " + world.Ry.ToString("g6"))
            text.AppendLine("  Rz: " + world.Rz.ToString("g6"))
            text.AppendLine(" Area Moments of Inertia about Centroid Coordinate Axes:")
            text.AppendLine("  Ix: " + centroid.Ix.ToString("g6"))
            text.AppendLine("  Iy: " + centroid.Iy.ToString("g6"))
            text.AppendLine("  Iz: " + centroid.Iz.ToString("g6"))
            text.AppendLine(" Area Radii of Gyration about Centroid Coordinate Axes")
            text.AppendLine("  Rx: " + centroid.Rx.ToString("g6"))
            text.AppendLine("  Ry: " + centroid.Ry.ToString("g6"))
            text.AppendLine("  Rz: " + centroid.Rz.ToString("g6"))
        End If
        Dim rf As New DetailsWindow()
        rf.Title = "Area Properties"
        rf.contentTextBox.Text = text.ToString()
        rf.Show()
    End Sub
    Private Sub volumeButton_OnClick(sender As Object, e As RoutedEventArgs)
        Dim vp As New VolumeProperties()
        Dim count As Integer = 0
        For i As Integer = 0 To viewportLayout1.Entities.Count - 1
            Dim ent As Entity = viewportLayout1.Entities(i)
            If ent.Selected Then
                If TypeOf ent Is IFace Then
                    Dim itfFace As IFace = DirectCast(ent, IFace)
                    Dim meshes As Mesh() = itfFace.GetPolygonMeshes()
                    For Each mesh As Mesh In meshes
                        vp.Add(mesh.Vertices, mesh.Triangles)
                    Next
                    count += 1
                End If
            End If
        Next
        Dim text As New StringBuilder()
        text.AppendLine(count.ToString() + " entity(ies) selected")
        text.AppendLine("---------------------")
        If vp.Centroid IsNot Nothing Then
            Dim x As Double, y As Double, z As Double
            Dim xx As Double, yy As Double, zz As Double, xy As Double, zx As Double, yz As Double
            Dim world As MomentOfInertia, centroid As MomentOfInertia
            vp.GetResults(vp.Volume, vp.Centroid, x, y, z, xx,
                yy, zz, xy, zx, yz, world,
                centroid)
            text.AppendLine("Cumulative volume: " + vp.Volume.ToString() + " cubic " + viewportLayout1.Units.ToString().ToLower())
            text.AppendLine("Cumulative centroid: " + vp.Centroid.ToString())
            text.AppendLine("Cumulative volume moments:")
            text.AppendLine(" First moments")
            text.AppendLine("  x: " + x.ToString("g6"))
            text.AppendLine("  y: " + y.ToString("g6"))
            text.AppendLine("  z: " + z.ToString("g6"))
            text.AppendLine(" Second moments")
            text.AppendLine("  xx: " + xx.ToString("g6"))
            text.AppendLine("  yy: " + yy.ToString("g6"))
            text.AppendLine("  zz: " + zz.ToString("g6"))
            text.AppendLine(" Product moments")
            text.AppendLine("  xy: " + xx.ToString("g6"))
            text.AppendLine("  yz: " + yy.ToString("g6"))
            text.AppendLine("  zx: " + zz.ToString("g6"))
            text.AppendLine(" Volume Moments of Inertia about World Coordinate Axes")
            text.AppendLine("  Ix: " + world.Ix.ToString("g6"))
            text.AppendLine("  Iy: " + world.Iy.ToString("g6"))
            text.AppendLine("  Iz: " + world.Iz.ToString("g6"))
            text.AppendLine(" Volume Radii of Gyration about World Coordinate Axes")
            text.AppendLine("  Rx: " + world.Rx.ToString("g6"))
            text.AppendLine("  Ry: " + world.Ry.ToString("g6"))
            text.AppendLine("  Rz: " + world.Rz.ToString("g6"))
            text.AppendLine(" Volume Moments of Inertia about Centroid Coordinate Axes:")
            text.AppendLine("  Ix: " + centroid.Ix.ToString("g6"))
            text.AppendLine("  Iy: " + centroid.Iy.ToString("g6"))
            text.AppendLine("  Iz: " + centroid.Iz.ToString("g6"))
            text.AppendLine(" Volume Radii of Gyration about Centroid Coordinate Axes")
            text.AppendLine("  Rx: " + centroid.Rx.ToString("g6"))
            text.AppendLine("  Ry: " + centroid.Ry.ToString("g6"))
            text.AppendLine("  Rz: " + centroid.Rz.ToString("g6"))
        End If
        Dim rf As New DetailsWindow()
        rf.Title = "Volume Properties"
        rf.contentTextBox.Text = text.ToString()
        rf.Show()
    End Sub
#End Region
#Region "Imaging"
    Private Sub rasterCopyToClipboardButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.CopyToClipboardRaster()
    End Sub
    Private Sub rasterSaveButton_OnClick(sender As Object, e As RoutedEventArgs)
        Dim mySaveFileDialog As New SaveFileDialog()
        mySaveFileDialog.InitialDirectory = "."
        mySaveFileDialog.Filter = "Bitmap (*.bmp)|*.bmp|" + "Portable Network Graphics (*.png)|*.png|" + "Windows metafile (*.wmf)|*.wmf|" + "Enhanced Windows Metafile (*.emf)|*.emf"
        mySaveFileDialog.FilterIndex = 2
        mySaveFileDialog.RestoreDirectory = True
        Dim result As Nullable(Of Boolean) = mySaveFileDialog.ShowDialog()
        If result = True Then
            Select Case mySaveFileDialog.FilterIndex
                Case 1
                    viewportLayout1.WriteToFileRaster(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp)
                    Exit Select
                Case 2
                    viewportLayout1.WriteToFileRaster(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png)
                    Exit Select
                Case 3
                    viewportLayout1.WriteToFileRaster(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Wmf)
                    Exit Select
                Case 4
                    viewportLayout1.WriteToFileRaster(2, mySaveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Emf)
                    Exit Select
            End Select
        End If
    End Sub
    Private Sub printButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.Print()
    End Sub
    Private Sub printPreviewButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.PrintPreview(New System.Drawing.Size(500, 400))
    End Sub
    Private Sub pageSetupButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.PageSetup()
    End Sub
    Private Sub vectorCopyToClipbardButton_OnClick(sender As Object, e As RoutedEventArgs)
        viewportLayout1.CopyToClipboardVector(False)
        'release mouse capture, otherwise the first mouse click is skipped
        vectorCopyToClipbardButton.ReleaseMouseCapture()
    End Sub
    Private Sub vectorSaveButton_OnClick(sender As Object, e As RoutedEventArgs)
        Dim mySaveFileDialog As New Microsoft.Win32.SaveFileDialog()
        mySaveFileDialog.Filter = "Enhanced Windows Metafile (*.emf)|*.emf"
        mySaveFileDialog.RestoreDirectory = True
        ' Show save file dialog box        
        If mySaveFileDialog.ShowDialog() = True Then
            ' To save as dxf/dwg, see the class HiddenLinesViewOnFileAutodesk available in x86 and x64 dlls                
            viewportLayout1.WriteToFileVector(False, mySaveFileDialog.FileName)
            'release mouse capture, otherwise the first mouse click is skipped                        
            vectorSaveButton.ReleaseMouseCapture()
        End If
    End Sub
#End Region
#Region "File"
    Private Sub websiteButton_OnClick(sender As Object, e As RoutedEventArgs)
        System.Diagnostics.Process.Start("www.devdept.com")
    End Sub
#End Region
#Region "Event handlers"
    Private Sub viewportLayout1_SelectionChanged(sender As Object, e As EventArgs) Handles viewportLayout1.SelectionChanged
        Dim count As Integer = 0
        ' counts selected entities
        For Each ent As Entity In viewportLayout1.Entities
            If ent.Selected Then
                count += 1
            End If
        Next
        Dim selected As Object() = New Object(count - 1) {}
        count = 0
        ' fills selected array
        For Each ent As Entity In viewportLayout1.Entities
            If ent.Selected Then
                selected(count) = ent
                count += 1
            End If
        Next
        ' updates count on the status bar
        selectedCountStatusLabel.Text = count.ToString()
        ' updates the propertyGrid control
        If selected.Length > 0 Then
            SetPropertyGridObject(selected(0))
        Else
            SetPropertyGridObject(Nothing)
        End If
    End Sub
    Private Sub SetPropertyGridObject(obj As Object)
        propertyGrid1.SelectedObject = obj
        If obj Is Nothing Then
            propertyGrid1.Visibility = Visibility.Collapsed
        Else
            propertyGrid1.Visibility = Visibility.Visible
        End If
    End Sub
    Private Sub viewportZero_LabelSelectionChanged(sender As Object, e As EventArgs)
        Dim count As Integer = 0
        ' counts selected entities
        For Each lbl As devDept.Eyeshot.Labels.Label In viewportLayout1.Viewports(0).Labels
            If lbl.Selected Then
                count += 1
            End If
        Next
        Dim selected As Object() = New Object(count - 1) {}
        count = 0
        ' fills selected array
        For Each lbl As devDept.Eyeshot.Labels.Label In viewportLayout1.Viewports(0).Labels
            If lbl.Selected Then
                selected(count) = lbl
                count += 1
            End If
        Next
        ' updates count on the status bar
        selectedCountStatusLabel.Text = count.ToString()
        ' updates the propertyGrid control
        If selected.Length > 0 Then
            SetPropertyGridObject(selected(0))
        Else
            SetPropertyGridObject(Nothing)
        End If
    End Sub
    Private Sub viewportLayout1_WorkCancelled(sender As Object, e As EventArgs) Handles viewportLayout1.WorkCancelled
        tabControl1.IsEnabled = True
        importButton.IsEnabled = True
        numericResultsButton.IsEnabled = False
        plotTypeComboBox.IsEnabled = False
    End Sub
    Private Sub viewportLayout1_WorkCompleted(sender As Object, e As devDept.Eyeshot.WorkCompletedEventArgs) Handles viewportLayout1.WorkCompleted
        If TypeOf e.WorkUnit Is ReadFileAsync Then
            Dim rfa As ReadFileAsync = DirectCast(e.WorkUnit, ReadFileAsync)
            rfa.AddToScene(viewportLayout1)
            viewportLayout1.ZoomFit()
        ElseIf TypeOf e.WorkUnit Is SolverBase Then
            Dim solver As SolverBase = DirectCast(e.WorkUnit, SolverBase)
            Dim fm As FemMesh = solver.Mesh
            ' computes the selected plot
            'fm.PlotMode = femPlotType.VonMises;
            fm.PlotMode = FemMesh.plotType.U
            fm.NodalAverages = True
            fm.ComputePlot(viewportLayout1, viewportLayout1.GetLegends()(0))
            viewportLayout1.ZoomFit()
        End If
        tabControl1.IsEnabled = True
        importButton.IsEnabled = True
        numericResultsButton.IsEnabled = True
        plotTypeComboBox.IsEnabled = True
        FillPlotTypeComboBox()
        UpdateDisplayModeButtons()
    End Sub
    Private Sub viewportLayout1_WorkFailed(sender As Object, e As WorkFailedEventArgs) Handles viewportLayout1.WorkFailed
        tabControl1.IsEnabled = True
        importButton.IsEnabled = True
        numericResultsButton.IsEnabled = False
        plotTypeComboBox.IsEnabled = False
    End Sub
#End Region
#Region "Fem"
    Private Sub plotTypeComboBox_OnSelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Draw.fm Is Nothing OrElse plotTypeComboBox.SelectedValue Is Nothing Then
            Return
        End If
        If viewportLayout1.GetLegends().Count > 0 Then
            viewportLayout1.GetLegends()(0).Visible = True
        End If
        Draw.fm.ContourPlot = True
        viewportLayout1.GetLegends()(0).ColorTable = Legend.RedToBlue17
        Select Case plotTypeComboBox.SelectedValue.ToString()
            Case "Mesh"
                Draw.fm.PlotMode = FemMesh.plotType.Mesh
                viewportLayout1.GetLegends()(0).Visible = False
                Exit Select
            Case "Ux"
                Draw.fm.PlotMode = FemMesh.plotType.Ux
                Draw.fm.ContourPlot = False
                Exit Select
            Case "Uy"
                Draw.fm.PlotMode = FemMesh.plotType.Uy
                Draw.fm.ContourPlot = False
                Exit Select
            Case "Uz"
                Draw.fm.PlotMode = FemMesh.plotType.Uz
                Draw.fm.ContourPlot = False
                Exit Select
            Case "U"
                Draw.fm.PlotMode = FemMesh.plotType.U
                Draw.fm.ContourPlot = False
                Exit Select
            Case "Rx"
                Draw.fm.PlotMode = FemMesh.plotType.Rx
                Exit Select
            Case "Ry"
                Draw.fm.PlotMode = FemMesh.plotType.Ry
                Exit Select
            Case "Rz"
                Draw.fm.PlotMode = FemMesh.plotType.Rz
                Exit Select
            Case "Axial Force"
                Draw.fm.PlotMode = FemMesh.plotType.AxialForce
                Exit Select
            Case "Shear Force V"
                Draw.fm.PlotMode = FemMesh.plotType.ShearForceV
                Exit Select
            Case "Shear Force W"
                Draw.fm.PlotMode = FemMesh.plotType.ShearForceW
                Exit Select
            Case "Torsion Moment"
                Draw.fm.PlotMode = FemMesh.plotType.TorsionMoment
                Exit Select
            Case "Bending Moment V"
                Draw.fm.PlotMode = FemMesh.plotType.BeamBendingMomentV
                Exit Select
            Case "Bending Moment W"
                Draw.fm.PlotMode = FemMesh.plotType.BeamBendingMomentW
                Exit Select
            Case "Twist Angle"
                Draw.fm.PlotMode = FemMesh.plotType.TwistAngle
                Exit Select
            Case "X stress"
                Draw.fm.PlotMode = FemMesh.plotType.Sx
                Exit Select
            Case "Y stress"
                Draw.fm.PlotMode = FemMesh.plotType.Sy
                Exit Select
            Case "Z stress"
                Draw.fm.PlotMode = FemMesh.plotType.Sz
                Exit Select
            Case "XY shear"
                Draw.fm.PlotMode = FemMesh.plotType.Txy
                Exit Select
            Case "YZ shear"
                Draw.fm.PlotMode = FemMesh.plotType.Tyz
                Exit Select
            Case "XZ shear"
                Draw.fm.PlotMode = FemMesh.plotType.Txz
                Exit Select
            Case "Maximum Principal"
                Draw.fm.PlotMode = FemMesh.plotType.P1
                Exit Select
            Case "Intermediate Principal"
                Draw.fm.PlotMode = FemMesh.plotType.P2
                Exit Select
            Case "Minimum Principal"
                Draw.fm.PlotMode = FemMesh.plotType.P3
                Exit Select
            Case "Von Mises"
                Draw.fm.PlotMode = FemMesh.plotType.VonMises
                Exit Select
            Case "Tresca"
                Draw.fm.PlotMode = FemMesh.plotType.Tresca
                Exit Select
        End Select
        ' re-computes the plot and redraw
        If Draw.fm IsNot Nothing Then
            Draw.fm.ComputePlot(viewportLayout1, viewportLayout1.GetLegends()(0))
            viewportLayout1.Entities.UpdateBoundingBox()
        End If
        '
        '            // updates label positions
        '            if (Draw.fm != null && Draw.fm.MinNodeIndex != -1)
        '            {
        '
        '                Node minNode = ((Node)Draw.fm.Vertices[Draw.fm.MinNodeIndex]);
        '                Node maxNode = ((Node)Draw.fm.Vertices[Draw.fm.MaxNodeIndex]);
        '
        '                Draw.minLabel.AnchorPoint = new Point3D(
        '                    Draw.fm.Vertices[Draw.fm.MinNodeIndex].X + minNode.UX * Draw.fm.AmplificationFactor,
        '                    Draw.fm.Vertices[Draw.fm.MinNodeIndex].Y + minNode.UY * Draw.fm.AmplificationFactor, 0);
        '
        '                Draw.maxLabel.AnchorPoint = new Point3D(
        '                    Draw.fm.Vertices[Draw.fm.MaxNodeIndex].X + maxNode.UX * Draw.fm.AmplificationFactor,
        '                    Draw.fm.Vertices[Draw.fm.MaxNodeIndex].Y + maxNode.UY * Draw.fm.AmplificationFactor, 0);
        '
        '
        '            }
        '            
        viewportLayout1.Invalidate()
        contourPlotCheckBox.IsChecked = Draw.fm.ContourPlot
        nodalAveragesCheckBox.IsChecked = Draw.fm.NodalAverages
    End Sub
    Private Sub contourPlotCheckBox_CheckedChanged(sender As Object, e As EventArgs)
        ' re-computes the plot and redraw
        If Draw.fm IsNot Nothing Then
            Draw.fm.ContourPlot = contourPlotCheckBox.IsChecked.Value
            Draw.fm.ComputePlot(viewportLayout1, viewportLayout1.GetLegends()(0))
            viewportLayout1.Invalidate()
        End If
    End Sub
    Private Sub nodalAveragesCheckBox_CheckedChanged(sender As Object, e As EventArgs)
        ' re-computes the plot and redraw
        If Draw.fm IsNot Nothing Then
            If nodalAveragesCheckBox.IsChecked IsNot Nothing Then
                Draw.fm.NodalAverages = nodalAveragesCheckBox.IsChecked.Value
            End If
            Draw.fm.ComputePlot(viewportLayout1, viewportLayout1.GetLegends()(0))
            viewportLayout1.Invalidate()
        End If
    End Sub
    Private Sub amplificationFactorTrackBar_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Draw.fm IsNot Nothing Then
            Dim amp As Double = 0
            Select Case CInt(amplificationFactorTrackBar.Value)
                Case 4
                    amp = 2
                    '10;
                    Exit Select
                Case 3
                    amp = 1
                    '10;
                    Exit Select
                Case 2
                    amp = 1 / 2.0
                    '5;
                    Exit Select
                Case 1
                    amp = 1 / 5.0
                    '1;
                    Exit Select
                Case 0
                    amp = 1 / 10.0
                    '1/5.0;
                    Exit Select
                Case -1
                    amp = 0
                    '1/10.0;
                    Exit Select
            End Select
            Draw.fm.AmplificationFactor = Draw.fm.OptimalAmplificationFactor * amp
            ' updates bounding box
            viewportLayout1.Entities.UpdateBoundingBox()
            ' recompiles all entities that need it                
            Draw.fm.Compile(New CompileParams(viewportLayout1) With {
                .Legend = viewportLayout1.GetLegends()(0)
            })
            viewportLayout1.Invalidate()
        End If
    End Sub
    Private Sub symbolSizeTrackBar_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        If Draw.fm IsNot Nothing Then
            ' Draw.fm.SymbolSize = symbolSizeTrackBar.Value;
            Select Case CInt(symbolSizeTrackBar.Value)
                Case 0
                    Draw.fm.SymbolSize = 0
                    Exit Select
                Case 1
                    Draw.fm.SymbolSize = 0.1
                    Exit Select
                Case 2
                    Draw.fm.SymbolSize = 0.25
                    Exit Select
                Case 3
                    Draw.fm.SymbolSize = 0.5
                    Exit Select
                Case 4
                    Draw.fm.SymbolSize = 0.75
                    Exit Select
                Case 5
                    Draw.fm.SymbolSize = 1
                    Exit Select
            End Select
            '  viewportLayout1.AdjustNearAndFarPlanes();
            viewportLayout1.Invalidate()
        End If
    End Sub
    Private Sub showRestraintsCheckBox_CheckedChanged(sender As Object, e As RoutedEventArgs)
        If viewportLayout1 Is Nothing Then
            Return
        End If
        If showRestraintsCheckBox.IsChecked IsNot Nothing Then
            viewportLayout1.ShowRestraint = showRestraintsCheckBox.IsChecked.Value
        End If
        viewportLayout1.Invalidate()
    End Sub
    Private Sub showJointCheckBox_CheckedChanged(sender As Object, e As RoutedEventArgs)
        If viewportLayout1 Is Nothing Then
            Return
        End If
        If showJointCheckBox.IsChecked IsNot Nothing Then
            viewportLayout1.ShowJoint = showJointCheckBox.IsChecked.Value
        End If
        viewportLayout1.Invalidate()
    End Sub
    Private Sub showLoadCheckBox_CheckedChanged(sender As Object, e As RoutedEventArgs)
        If viewportLayout1 Is Nothing Then
            Return
        End If
        If showLoadCheckBox.IsChecked IsNot Nothing Then
            viewportLayout1.ShowLoad = showLoadCheckBox.IsChecked.Value
        End If
        viewportLayout1.Invalidate()
    End Sub
    Private Sub showVertexIndicesCheckBox_CheckedChanged(sender As Object, e As RoutedEventArgs)
        If viewportLayout1 Is Nothing Then
            Return
        End If
        If showVertexIndicesCheckBox.IsChecked IsNot Nothing Then
            viewportLayout1.ShowVertexIndices = showVertexIndicesCheckBox.IsChecked.Value
        End If
        viewportLayout1.Invalidate()
    End Sub
#End Region
    Private Sub propertyGrid1_PropertyValueChanged(sender As Object, e As Xceed.Wpf.Toolkit.PropertyGrid.PropertyValueChangedEventArgs)
        ' Update the entities
        viewportLayout1.Entities.Regen()
        ' Redraw
        viewportLayout1.Invalidate()
    End Sub
    Private Sub importButton_OnClick(sender As Object, e As RoutedEventArgs)
        Dim openFileDialog1 As New OpenFileDialog()
        Dim theFilter As String = "Points|*.asc|" + "Stereolithography|*.stl|" + "WaveFront OBJ|*.obj"
        openFileDialog1.Filter = theFilter
        openFileDialog1.Multiselect = False
        openFileDialog1.AddExtension = True
        openFileDialog1.CheckFileExists = True
        openFileDialog1.CheckPathExists = True
        Dim result As Nullable(Of Boolean) = openFileDialog1.ShowDialog()
        If result = True Then
            viewportLayout1.Entities.Clear()
            Dim rfa As ReadFileAsync = Nothing
            Select Case openFileDialog1.FilterIndex
                Case 1
                    rfa = New ReadASC(openFileDialog1.FileName)
                    Exit Select
                Case 2
                    rfa = New devDept.Eyeshot.Translators.ReadSTL(openFileDialog1.FileName)
                    Exit Select
                Case 3
                    rfa = New devDept.Eyeshot.Translators.ReadOBJ(openFileDialog1.FileName)
                    Exit Select
            End Select
            viewportLayout1.StartWork(rfa)
            viewportLayout1.SetView(viewType.Trimetric, True, viewportLayout1.AnimateCamera)
            importButton.IsEnabled = False
        End If
    End Sub
    Private Sub numericResultsButton_OnClick(sender As Object, e As RoutedEventArgs)
        If Draw.fm IsNot Nothing Then
            Dim nr As New NumericResultsWindow(Draw.fm)
            nr.Show()
        End If
    End Sub
    Private Sub Window_Closed(sender As Object, e As EventArgs)
        For Each win As Window In Application.Current.Windows
            win.Close()
        Next
    End Sub
End Class
