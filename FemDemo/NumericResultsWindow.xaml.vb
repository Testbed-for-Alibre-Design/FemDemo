Imports System.Collections.ObjectModel
Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Media
Imports devDept.Eyeshot.Entities
Imports devDept.Eyeshot.Fem
Imports devDept.Geometry
Imports Microsoft.Win32

''' <summary>
''' Interaction logic for DetailsWindow.xaml
''' </summary>
Partial Public Class NumericResultsWindow    
    Private _femMesh As FemMesh
    Private _elementsType As FemElementsType

    Private _numericResults As ObservableCollection(Of NumericResultsItem)
    Private ReadOnly _numericResultsItemType As Type = GetType(NumericResultsItem)

    Public Sub New(femMesh As FemMesh)
        InitializeComponent()

        _femMesh = femMesh
        _elementsType = MainWindow.GetFemElementsType(_femMesh)

        If _femMesh IsNot Nothing AndAlso _elementsType <> FemElementsType.None Then
            FillListView()
        End If
    End Sub

    Private Sub FillListView()
        ' Clear all items and columns            
        resultsGridView.Columns.Clear()
        _numericResults = New ObservableCollection(Of NumericResultsItem)()

        If nodeRadioBtn.IsChecked.HasValue AndAlso nodeRadioBtn.IsChecked.Value Then
            For i As Integer = 0 To _femMesh.Vertices.Length - 1
                Dim vertex = _femMesh.Vertices(i)

                FillVertexValue(vertex, i, 0, vertex)

            Next
        Else
            'Element values
            For i As Integer = 0 To _femMesh.Elements.Length - 1
                Dim element = _femMesh.Elements(i)

                For j As Integer = 0 To element.Connection.Length - 1
                    Dim connection = element.Connection(j)
                    Dim vertex = _femMesh.Vertices(connection)

                    Dim item As NumericResultsItem
                    If materialCheckBox.IsChecked.HasValue AndAlso materialCheckBox.IsChecked.Value Then
                        'Fill Material Column
                        item = AddItem("Material", element.Material.Description)
                        'Fill Element Column
                        AddItem("Element", i.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                    Else
                        'Fill Element Column
                        item = AddItem("Element", i.ToString(CultureInfo.InvariantCulture))
                    End If
                    'Fill Local node Column
                    AddItem("LocalNode", j.ToString(CultureInfo.InvariantCulture), "Local node", item)
                    FillVertexValue(vertex, connection, j, element, item)
                Next
            Next
        End If

        'Add the items to the ListView.
        itemsListView.ItemsSource = _numericResults

        itemsListView.Items.Refresh()

        'Set the total rows number
        totalRowLabel.Content = "Total rows: " + itemsListView.Items.Count.ToString(CultureInfo.InvariantCulture)

    End Sub

    Private Sub FillVertexValue(vertex As Point3D, vertexIndex As Integer, localIndex As Integer, baseObject As Object, Optional mainItem As NumericResultsItem = Nothing)
        'Node reactions prefilter
        If nodeReactionsCheckBox.IsChecked.HasValue AndAlso nodeReactionsCheckBox.IsChecked.Value Then
            Select Case _elementsType
                Case FemElementsType.Beam2D, FemElementsType.Beam3D
                    Dim nodeBeam = DirectCast(baseObject, NodeBeam)
                    If nodeBeam.Reactions Is Nothing Then
                        Return
                    End If
                    Exit Select
                Case FemElementsType.Element2D, FemElementsType.Element3D
                    Dim node = DirectCast(baseObject, Node)
                    If node.Reactions Is Nothing Then
                        Return
                    End If
                    Exit Select
                Case Else
                    Return
            End Select
        End If

        Dim item As NumericResultsItem = mainItem

        'Fill Node Column
        If item Is Nothing Then
            item = AddItem("Node", vertexIndex.ToString(CultureInfo.InvariantCulture))
        Else
            AddItem("Node", vertexIndex.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
        End If

        'Fill Node coordinates Column
        If nodeCoordCheckBox.IsChecked.HasValue AndAlso nodeCoordCheckBox.IsChecked.Value Then
            AddItem("X", vertex.X.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
            AddItem("Y", vertex.Y.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
            AddItem("Z", vertex.Z.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
        End If


        If nodeRadioBtn.IsChecked.HasValue AndAlso nodeRadioBtn.IsChecked.Value Then
            Select Case _elementsType
                Case FemElementsType.Beam2D, FemElementsType.Beam3D
                    Dim nodeBeam = DirectCast(baseObject, NodeBeam)

                    AddItem("Ux", nodeBeam.Ux.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                    AddItem("Uy", nodeBeam.Uy.ToString(CultureInfo.InvariantCulture), [String].Empty, item)

                    If _elementsType = FemElementsType.Beam3D Then
                        AddItem("Uz", nodeBeam.Uz.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                    End If

                    If nodeReactionsCheckBox.IsChecked.HasValue AndAlso nodeReactionsCheckBox.IsChecked.Value Then
                        AddItem("ReactionX", nodeBeam.Reactions(0).ToString(CultureInfo.InvariantCulture), "Reaction X", item)
                        AddItem("ReactionY", nodeBeam.Reactions(1).ToString(CultureInfo.InvariantCulture), "Reaction Y", item)

                        If _elementsType = FemElementsType.Beam3D Then
                            AddItem("ReactionZ", nodeBeam.Reactions(2).ToString(CultureInfo.InvariantCulture), "Reaction Z", item)
                            AddItem("MomentAboutX", nodeBeam.Reactions(3).ToString(CultureInfo.InvariantCulture), "Moment about X", item)
                            AddItem("MomentAboutY", nodeBeam.Reactions(4).ToString(CultureInfo.InvariantCulture), "Moment about Y", item)
                            AddItem("MomentAboutZ", nodeBeam.Reactions(5).ToString(CultureInfo.InvariantCulture), "Moment about Z", item)
                        Else
                            AddItem("MomentAboutZ", nodeBeam.Reactions(2).ToString(CultureInfo.InvariantCulture), "Moment about Z", item)

                        End If
                    Else
                        'nodeReactionsCheckBox unchecked
                        AddItem("U", nodeBeam.U.ToString(CultureInfo.InvariantCulture), [String].Empty, item)

                        If _elementsType = FemElementsType.Beam3D Then
                            AddItem("Rx", nodeBeam.Rx.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                            AddItem("Ry", nodeBeam.Ry.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                        End If

                        AddItem("Rz", nodeBeam.Rz.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                    End If
                    Exit Select

                Case FemElementsType.Element2D, FemElementsType.Element3D
                    Dim node = DirectCast(baseObject, Node)

                    AddItem("Ux", node.Ux.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                    AddItem("Uy", node.Uy.ToString(CultureInfo.InvariantCulture), [String].Empty, item)

                    If _elementsType = FemElementsType.Element3D Then
                        AddItem("Uz", node.Uz.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                    End If

                    If nodeReactionsCheckBox.IsChecked.HasValue AndAlso nodeReactionsCheckBox.IsChecked.Value Then
                        AddItem("ReactionX", node.Reactions(0).ToString(CultureInfo.InvariantCulture), "Reaction X", item)
                        AddItem("ReactionY", node.Reactions(1).ToString(CultureInfo.InvariantCulture), "Reaction Y", item)
                        If _elementsType = FemElementsType.Beam3D Then
                            AddItem("ReactionZ", node.Reactions(2).ToString(CultureInfo.InvariantCulture), "Reaction Z", item)
                        End If
                    Else
                        'nodeReactionsCheckBox unchecked
                        AddItem("U", node.U.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                        AddItem("XStress", node.Sx.ToString(CultureInfo.InvariantCulture), "X stress", item)
                        AddItem("YStress", node.Sy.ToString(CultureInfo.InvariantCulture), "Y stress", item)

                        If _elementsType = FemElementsType.Element3D Then
                            AddItem("ZStress", node.Sz.ToString(CultureInfo.InvariantCulture), "Z stress", item)
                        End If


                        AddItem("XyShear", node.Txy.ToString(CultureInfo.InvariantCulture), "XY shear", item)

                        If _elementsType = FemElementsType.Element3D Then
                            AddItem("YzShear", node.Tyz.ToString(CultureInfo.InvariantCulture), "YZ shear", item)
                            AddItem("XzShear", node.Txz.ToString(CultureInfo.InvariantCulture), "XZ shear", item)
                        End If

                        AddItem("MaximumPrincipal", node.P1.ToString(CultureInfo.InvariantCulture), "Maximum Principal", item)
                        AddItem("IntermediatePrincipal", node.P2.ToString(CultureInfo.InvariantCulture), "Intermediate Principal", item)

                        If _elementsType = FemElementsType.Element3D Then
                            AddItem("MinimumPrincipal", node.P3.ToString(CultureInfo.InvariantCulture), "Minimum Principal", item)
                        End If

                        AddItem("VonMises", node.VonMises.ToString(CultureInfo.InvariantCulture), "Von mises", item)
                        AddItem("Tresca", node.Tresca.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                    End If
                    Exit Select
            End Select
        Else
            'Element values
            Select Case _elementsType
                Case FemElementsType.Beam2D
                    Dim beam2D = DirectCast(baseObject, Beam2D)

                    AddItem("AxialForce", beam2D.AxialForce(localIndex).ToString(CultureInfo.InvariantCulture), "Axial Force", item)
                    AddItem("ShearForceV", beam2D.ShearForce(localIndex).ToString(CultureInfo.InvariantCulture), "Shear Force V", item)
                    AddItem("BendingMomentW", beam2D.BendingMoment(localIndex).ToString(CultureInfo.InvariantCulture), "Bending Moment W", item)
                    Exit Select

                Case FemElementsType.Beam3D
                    Dim beam3D = DirectCast(baseObject, Beam)

                    AddItem("AxialForce", beam3D.AxialForce(localIndex).ToString(CultureInfo.InvariantCulture), "Axial Force", item)
                    AddItem("ShearForceV", beam3D.ShearForceV(localIndex).ToString(CultureInfo.InvariantCulture), "Shear Force V", item)
                    AddItem("ShearForceW", beam3D.ShearForceW(localIndex).ToString(CultureInfo.InvariantCulture), "Shear Force W", item)
                    AddItem("TorsionMoment", beam3D.TorsionMoment.ToString(CultureInfo.InvariantCulture), "Torsion Moment", item)
                    AddItem("BendingMomentV", beam3D.BendingMomentV(localIndex).ToString(CultureInfo.InvariantCulture), "Bending Moment V", item)
                    AddItem("BendingMomentW", beam3D.BendingMomentW(localIndex).ToString(CultureInfo.InvariantCulture), "Bending Moment W", item)
                    AddItem("TwistAngle", beam3D.TwistAngle(localIndex).ToString(CultureInfo.InvariantCulture), "Twist Angle", item)

                    Exit Select

                Case FemElementsType.Element2D, FemElementsType.Element3D
                    Dim el = DirectCast(baseObject, Element)

                    AddItem("XStress", el.Stress(localIndex, 0).ToString(CultureInfo.InvariantCulture), "X stress", item)
                    AddItem("YStress", el.Stress(localIndex, 1).ToString(CultureInfo.InvariantCulture), "Y stress", item)

                    If _elementsType = FemElementsType.Element3D Then
                        AddItem("ZStress", el.Stress(localIndex, 2).ToString(CultureInfo.InvariantCulture), "Z stress", item)
                    End If

                    AddItem("XyShear", el.Stress(localIndex, 3).ToString(CultureInfo.InvariantCulture), "XY shear", item)

                    If _elementsType = FemElementsType.Element3D Then
                        AddItem("YzShear", el.Stress(localIndex, 4).ToString(CultureInfo.InvariantCulture), "YZ shear", item)
                        AddItem("XzShear", el.Stress(localIndex, 5).ToString(CultureInfo.InvariantCulture), "XZ shear", item)
                    End If

                    AddItem("MaximumPrincipal", el.Principals(localIndex, 0).ToString(CultureInfo.InvariantCulture), "Maximum Principal", item)
                    AddItem("IntermediatePrincipal", el.Principals(localIndex, 1).ToString(CultureInfo.InvariantCulture), "Intermediate Principal", item)

                    If _elementsType = FemElementsType.Element3D Then
                        AddItem("MinimumPrincipal", el.Principals(localIndex, 2).ToString(CultureInfo.InvariantCulture), "Minimum Principal", item)
                    End If

                    AddItem("VonMises", el.VonMises(localIndex).ToString(CultureInfo.InvariantCulture), "Von mises", item)

                    Dim tresca = el.Principals(localIndex, 2) - el.Principals(localIndex, 0)
                    AddItem("Tresca", tresca.ToString(CultureInfo.InvariantCulture), [String].Empty, item)
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub AddColumn(propertyName As String, Optional caption As String = "")
        Dim columnCaption As String = caption
        If [String].IsNullOrEmpty(caption) Then
            columnCaption = propertyName
        End If

        Dim columnsCount = Aggregate col In resultsGridView.Columns
                                  Where col.Header = columnCaption
                                  Into Count()

        If columnsCount = 0 Then
            resultsGridView.Columns.Add(New GridViewColumn() With { _
                .DisplayMemberBinding = New Binding(propertyName), _
                .Header = columnCaption, _
                .Width = 100 _
            })
        End If

    End Sub

    Private Function AddItem(propertyName As String, value As String, Optional columnCaption As String = "", Optional mainItem As NumericResultsItem = Nothing) As NumericResultsItem
        AddColumn(propertyName, columnCaption)

        If mainItem Is Nothing Then
            mainItem = New NumericResultsItem()
            _numericResults.Add(mainItem)
        End If

        Dim prop As PropertyInfo = _numericResultsItemType.GetProperty(propertyName)
        prop.SetValue(mainItem, value, Nothing)

        Return mainItem
    End Function

    Private Sub viewType_CheckedChanged(sender As Object, e As RoutedEventArgs)
        Dim materialEnabled As Boolean = nodeRadioBtn.IsChecked IsNot Nothing AndAlso Not nodeRadioBtn.IsChecked.Value
        If materialCheckBox IsNot Nothing Then
            materialCheckBox.IsEnabled = materialEnabled
            If Not materialEnabled Then
                materialCheckBox.IsChecked = False
            End If

            nodeReactionsCheckBox.IsEnabled = nodeRadioBtn.IsChecked IsNot Nothing AndAlso nodeRadioBtn.IsChecked.Value
            If Not nodeReactionsCheckBox.IsEnabled Then
                nodeReactionsCheckBox.IsChecked = False
            End If
        End If

    End Sub

    Private Sub UpdateButton_OnClick(sender As Object, e As RoutedEventArgs)
        FillListView()
    End Sub

    Private Sub closeButton_Click(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub

    Private Sub csvButton_OnClick(sender As Object, e As RoutedEventArgs)
        'declare new SaveFileDialog and set it's initial properties
        If True Then
            Dim sfd = New SaveFileDialog() With { _
                 .Title = "Choose file to save to", _
                .FileName = "", _
                .Filter = "CSV (*.csv)|*.csv", _
                .FilterIndex = 0, _
                .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) _
            }

            'show the dialog and display the results in a msgbox unless cancelled                
            Dim result As Nullable(Of Boolean) = sfd.ShowDialog()
            If result = True Then
                Using stream As Stream = File.Open(sfd.FileName, FileMode.Create, FileAccess.Write)
                    Dim textWriter As TextWriter = New StreamWriter(stream, Encoding.ASCII)

                    Dim columnsCount As Integer = resultsGridView.Columns.Count

                    'Header
                    For i As Integer = 0 To columnsCount - 1
                        Dim column = resultsGridView.Columns(i)
                        textWriter.Write(column.Header)
                        If i = columnsCount - 1 Then
                            textWriter.Write(Environment.NewLine)
                        Else
                            textWriter.Write(",")
                        End If
                    Next

                    'Body                          
                    For i As Integer = 0 To itemsListView.Items.Count - 1
                        Dim t As Type = itemsListView.Items(i).[GetType]()
                        For j As Integer = 0 To resultsGridView.Columns.Count - 1
                            Dim gc As GridViewColumn = resultsGridView.Columns(j)
                            Dim bindingPath As String = DirectCast(gc.DisplayMemberBinding, Binding).Path.Path
                            Dim pi As PropertyInfo = itemsListView.Items(i).[GetType]().GetProperty(bindingPath)
                            Dim value = pi.GetValue(itemsListView.Items(i), Nothing)
                            If value IsNot Nothing Then
                                Dim textValue As String = value.ToString()
                                textWriter.Write(textValue)
                            End If
                            If j = columnsCount - 1 Then
                                textWriter.Write(Environment.NewLine)
                            Else
                                textWriter.Write(",")
                            End If
                        Next
                    Next

                    textWriter.Close()
                End Using


                MessageBox.Show("Export Complete!")
            End If
        End If
    End Sub
End Class

''' <summary>    
''' This class represent the Model for NumericResults ListView.
''' </summary>    
Public Class NumericResultsItem
    Public Property Material() As String
        Get
            Return m_Material
        End Get
        Set(value As String)
            m_Material = value
        End Set
    End Property
    Private m_Material As String
    Public Property Element() As String
        Get
            Return m_Element
        End Get
        Set(value As String)
            m_Element = value
        End Set
    End Property
    Private m_Element As String
    Public Property LocalNode() As String
        Get
            Return m_LocalNode
        End Get
        Set(value As String)
            m_LocalNode = value
        End Set
    End Property
    Private m_LocalNode As String
    Public Property Node() As String
        Get
            Return m_Node
        End Get
        Set(value As String)
            m_Node = value
        End Set
    End Property
    Private m_Node As String
    Public Property X() As String
        Get
            Return m_X
        End Get
        Set(value As String)
            m_X = value
        End Set
    End Property
    Private m_X As String
    Public Property Y() As String
        Get
            Return m_Y
        End Get
        Set(value As String)
            m_Y = value
        End Set
    End Property
    Private m_Y As String
    Public Property Z() As String
        Get
            Return m_Z
        End Get
        Set(value As String)
            m_Z = value
        End Set
    End Property
    Private m_Z As String
    Public Property Ux() As String
        Get
            Return m_Ux
        End Get
        Set(value As String)
            m_Ux = value
        End Set
    End Property
    Private m_Ux As String
    Public Property Uy() As String
        Get
            Return m_Uy
        End Get
        Set(value As String)
            m_Uy = value
        End Set
    End Property
    Private m_Uy As String
    Public Property Uz() As String
        Get
            Return m_Uz
        End Get
        Set(value As String)
            m_Uz = value
        End Set
    End Property
    Private m_Uz As String
    Public Property U() As String
        Get
            Return m_U
        End Get
        Set(value As String)
            m_U = value
        End Set
    End Property
    Private m_U As String
    Public Property Rx() As String
        Get
            Return m_Rx
        End Get
        Set(value As String)
            m_Rx = value
        End Set
    End Property
    Private m_Rx As String
    Public Property Ry() As String
        Get
            Return m_Ry
        End Get
        Set(value As String)
            m_Ry = value
        End Set
    End Property
    Private m_Ry As String
    Public Property Rz() As String
        Get
            Return m_Rz
        End Get
        Set(value As String)
            m_Rz = value
        End Set
    End Property
    Private m_Rz As String
    Public Property XStress() As String
        Get
            Return m_XStress
        End Get
        Set(value As String)
            m_XStress = value
        End Set
    End Property
    Private m_XStress As String
    Public Property YStress() As String
        Get
            Return m_YStress
        End Get
        Set(value As String)
            m_YStress = value
        End Set
    End Property
    Private m_YStress As String
    Public Property ZStress() As String
        Get
            Return m_ZStress
        End Get
        Set(value As String)
            m_ZStress = value
        End Set
    End Property
    Private m_ZStress As String
    Public Property XyShear() As String
        Get
            Return m_XyShear
        End Get
        Set(value As String)
            m_XyShear = value
        End Set
    End Property
    Private m_XyShear As String
    Public Property YzShear() As String
        Get
            Return m_YzShear
        End Get
        Set(value As String)
            m_YzShear = value
        End Set
    End Property
    Private m_YzShear As String
    Public Property XzShear() As String
        Get
            Return m_XzShear
        End Get
        Set(value As String)
            m_XzShear = value
        End Set
    End Property
    Private m_XzShear As String
    Public Property MaximumPrincipal() As String
        Get
            Return m_MaximumPrincipal
        End Get
        Set(value As String)
            m_MaximumPrincipal = value
        End Set
    End Property
    Private m_MaximumPrincipal As String
    Public Property IntermediatePrincipal() As String
        Get
            Return m_IntermediatePrincipal
        End Get
        Set(value As String)
            m_IntermediatePrincipal = value
        End Set
    End Property
    Private m_IntermediatePrincipal As String
    Public Property MinimumPrincipal() As String
        Get
            Return m_MinimumPrincipal
        End Get
        Set(value As String)
            m_MinimumPrincipal = value
        End Set
    End Property
    Private m_MinimumPrincipal As String
    Public Property VonMises() As String
        Get
            Return m_VonMises
        End Get
        Set(value As String)
            m_VonMises = value
        End Set
    End Property
    Private m_VonMises As String
    Public Property Tresca() As String
        Get
            Return m_Tresca
        End Get
        Set(value As String)
            m_Tresca = value
        End Set
    End Property
    Private m_Tresca As String
    Public Property AxialForce() As String
        Get
            Return m_AxialForce
        End Get
        Set(value As String)
            m_AxialForce = value
        End Set
    End Property
    Private m_AxialForce As String
    Public Property ShearForceV() As String
        Get
            Return m_ShearForceV
        End Get
        Set(value As String)
            m_ShearForceV = value
        End Set
    End Property
    Private m_ShearForceV As String
    Public Property ShearForceW() As String
        Get
            Return m_ShearForceW
        End Get
        Set(value As String)
            m_ShearForceW = value
        End Set
    End Property
    Private m_ShearForceW As String
    Public Property BendingMomentV() As String
        Get
            Return m_BendingMomentV
        End Get
        Set(value As String)
            m_BendingMomentV = value
        End Set
    End Property
    Private m_BendingMomentV As String
    Public Property BendingMomentW() As String
        Get
            Return m_BendingMomentW
        End Get
        Set(value As String)
            m_BendingMomentW = value
        End Set
    End Property
    Private m_BendingMomentW As String
    Public Property TorsionMoment() As String
        Get
            Return m_TorsionMoment
        End Get
        Set(value As String)
            m_TorsionMoment = value
        End Set
    End Property
    Private m_TorsionMoment As String
    Public Property TwistAngle() As String
        Get
            Return m_TwistAngle
        End Get
        Set(value As String)
            m_TwistAngle = value
        End Set
    End Property
    Private m_TwistAngle As String
    Public Property ReactionX() As String
        Get
            Return m_ReactionX
        End Get
        Set(value As String)
            m_ReactionX = value
        End Set
    End Property
    Private m_ReactionX As String
    Public Property ReactionY() As String
        Get
            Return m_ReactionY
        End Get
        Set(value As String)
            m_ReactionY = value
        End Set
    End Property
    Private m_ReactionY As String
    Public Property ReactionZ() As String
        Get
            Return m_ReactionZ
        End Get
        Set(value As String)
            m_ReactionZ = value
        End Set
    End Property
    Private m_ReactionZ As String
    Public Property MomentAboutX() As String
        Get
            Return m_MomentAboutX
        End Get
        Set(value As String)
            m_MomentAboutX = value
        End Set
    End Property
    Private m_MomentAboutX As String
    Public Property MomentAboutY() As String
        Get
            Return m_MomentAboutY
        End Get
        Set(value As String)
            m_MomentAboutY = value
        End Set
    End Property
    Private m_MomentAboutY As String
    Public Property MomentAboutZ() As String
        Get
            Return m_MomentAboutZ
        End Get
        Set(value As String)
            m_MomentAboutZ = value
        End Set
    End Property
    Private m_MomentAboutZ As String
End Class
