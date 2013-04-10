Log:Debug("Initializing Scene 2", nil)

node = AddAmbientLight(Vector4(0.2, 0.2, 0.2, 0.2), 0)

node = AddDirectionalLight(Vector4(1, 1, 0.5, 1), Vector3(0.5, -1, 1))
node = AddDirectionalLight(Vector4(1, 1, 1, 0.2), Vector3(-1, -1, -1))

node = AddModel("Models\\monkey.dae")
node.Scale = Vector3(1, 1, 1)
RotateNode(node, Vector3(1, 0, 0), -90)
ChangeMaterial(node, 1, true)

node = AddPlane()
node.Scale = Vector3(1000, 1000, 1000)
RotateNode(node, Vector3(1, 0, 0), 90)
ChangeMaterial(node, 3, true)
