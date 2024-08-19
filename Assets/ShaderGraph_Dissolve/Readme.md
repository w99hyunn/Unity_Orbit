##Dissolve ShaderGraph##



##HDRP v14.0.8



#Developped by nott



##Description


All shaders are created in ShaderGraph.
You can achieve more extensions through subgraph.
For instructions on SubGraph, please open the shadergraph panel to view.
# ATTENTION TO USER:
Directional dissolve needs to be achieved by controlling DissolveOffest or DissolveDirection.
Please confirm the coordinate system before use.
#Double Side
if you want use DoubleSided Mode, open ShardGraph and Enable DoubleSided Mode


##PARAMETERS




#BaseColor
#BaseMap
#NormalMap
#NormalScale
#Tiling
#Offest
#R_Metallic，G_Occulsion,A_Smoothness
Channel packing consistent with Unity

#Metallic
#OcclusionStrength
#Smoothness
#OcclusionMap
#OcclusionStength
#RGB_SpecularMap,A_Smoothness
Channel packing
RGB SpecularMap
A    Smoothness

#SpecularColor
#UseSpecularMap
Weight between SpecularColor and SpecularMap

#DissolveThe weight value of dissolving, 1 means all dissolving, and vice versa
#NoiseScale
Scaling of the noise mask

#NoiseUVSpeed
The flow velocity of the noise mask

#EdgeWidth
Dissolve boundary width

#EdgeColor
Dissolve boundary color

#EdgeColorIntensity
Color intensity of the border

#DissolveOffest
Dissolved offset point

#DissolveDirection
#DiertionEdgeWithScale
#DissolveDirection Is EulerAngle
The unit of DissolveDirection is Euler angle

#UseWorldSpace
The default is
Localspace

#UseWorldOrigin
It will only take effect when UseWorldSpace is turned on.
DissolveOffest origin is WorldOrigin