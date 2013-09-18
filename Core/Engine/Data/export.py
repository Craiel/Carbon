import bpy
import sys

outFile = sys.argv[-1:][0]
print("Exporting XCD to %s" % outFile)

bpy.ops.export_scene.xcd(filepath=outFile, check_existing=False)
sys.exit(0)
