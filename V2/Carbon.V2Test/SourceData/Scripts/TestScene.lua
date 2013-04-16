Log:Debug("Initializing Scene", nil)

node = AddAmbientLight(Vector4(0.2, 0.2, 0.2, 0.2), 0)

--node = AddDirectionalLight(Vector4(1, 1, 0.5, 1), Vector3(0.5, -1, 1))
node = AddDirectionalLight(Vector4(1, 1, 1, 0.2), Vector3(-1, -1, -1))

node = AddModel("Models\\WorldTest.dae")
node.Scale = Vector3(10, 10, 10)
RotateNode(node, Vector3(1, 0, 0), -90)
ChangeMaterial(node, 5, true)

node = AddPlane()
node.Scale = Vector3(1000, 1000, 1000)
RotateNode(node, Vector3(1, 0, 0), 90)
ChangeMaterial(node, 1, true)

node = AddStaticText(2, "Final Fantasy Font\n1 2 3 4 5 - ABCDE", Vector2(1, 1))
node.Position = Vector4(0, 12, 0, 1)

node = AddStaticText(1, "Arial Hi-Rez\n1 2 3 4 5 - ABCDE", Vector2(1, 1))
node.Position = Vector4(0, 16, 0, 1)

parentNode = AddNode()
sphereCount = 0
for x = 1, 1 do
	for y = 1, 20 do
	    sphereCount = sphereCount + 1
		node = AddSphere(2, parentNode)
		node.Position = Vector4(x, y, 10, 1)
		node.Scale = Vector3(0.2, 0.2, 0.2)
		ChangeMaterial(node, 2, true)
		
		node = AddStaticText(1, "Sphere: "..sphereCount, Vector2(0.06, 0.1))
		node.Position = Vector4(x - 0.2, y, 9.5, 1)
	
		end
	end
