Log:Debug("Initializing Scene", nil)

node = AddAmbientLight(Vector4(0.2, 0.2, 0.2, 0.2), 0)

node = AddDirectionalLight(Vector4(1, 1, 0.5, 1), Vector3(0.5, -1, 1))
node = AddDirectionalLight(Vector4(1, 1, 1, 0.2), Vector3(-1, -1, -1))

--node = AddPointLight(Vector4(0.5, 1, 1, 1), 20, 10)
--node.Position = Vector4(5, 1, 0, 1)

--node = AddPointLight(Vector4(1, 1, 1, 1), 20, 10)
--node.Position = Vector4(15, 1, 0, 1)

--node = AddPointLight(Vector4(1, 1, 1, 1), 20, 10)
--node.Position = Vector4(10, 1, 5, 1)

--local spotColor = Vector4(1, 0, 1, 1)
--node = AddSpotLight(spotColor, Vector2(5, 10), Vector3(0, 1, 0), 10)
--node.Position = Vector4(15, 5, 20, 1)

--node = AddSpotLight(spotColor, Vector2(5, 10), Vector3(0, 1, 0), 10)
--node.Position = Vector4(25, 2, 20, 1)

--node = AddSpotLight(spotColor, Vector2(5, 10), Vector3(0, 1, 0), 10)
--node.Position = Vector4(35, 0.5, 20, 1)

node = AddModel("Models\\rock_4.dae")
node.Position = Vector4(0, 1, -2, 1)

node = AddModel("Models\\House6.dae")
node.Position = Vector4(5, 1, 0, 1)

node = AddModel("Models\\House.dae")
node.Position = Vector4(-7, 1, -6, 1)

node = AddModel("Models\\monkey.dae")
node.Position = Vector4(-5, 1, 0, 1)
ChangeMaterial(node, 3, true)

node = AddSphere(2, parentNode)
node.Position = Vector4(-5, 2, 5, 1)
ChangeMaterial(node, 3, true)

parentNode = AddNode()
for x = 1, 5 do
	for y = 1, 10 do
		node = AddSphere(2, parentNode)
		node.Position = Vector4(x, y, 10, 1)
		node.Scale = Vector3(0.2, 0.2, 0.2)
		ChangeMaterial(node, 1, true)
		end
	end
