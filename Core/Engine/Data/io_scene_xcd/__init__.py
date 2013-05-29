# ##### BEGIN LICENSE BLOCK #####
#
#  @PG, Carbon
#
# ##### END LICENSE BLOCK #####

# <pep8-80 compliant>

bl_info = {
    "name": "Carbon Stage Format",
    "author": "PG",
    "blender": (2, 66, 0),
    "location": "File > Export",
    "description": "Export Carbon XCD",
    "warning": "",
    "wiki_url": "",
    "tracker_url": "",
    "support": 'TESTING',
    "category": "Export"}

if "bpy" in locals():
    import imp
    if "export_xcd" in locals():
        imp.reload(export_xcd)

import bpy
from bpy.props import StringProperty, BoolProperty, EnumProperty
from bpy_extras.io_utils import (ExportHelper,
                                 axis_conversion,
                                 path_reference_mode,
                                 )

class ExportXCD(bpy.types.Operator, ExportHelper):
    """Export selection to Stage file (.xcd)"""
    bl_idname = "export_scene.xcd"
    bl_label = 'Export XCD'
    bl_options = {'PRESET'}

    filename_ext = ".xcd"
    filter_glob = StringProperty(default="*.xcd", options={'HIDDEN'})
    
    use_selection = BoolProperty(
            name="Selection Only",
            description="Export selected objects only",
            default=False,
            )

    def execute(self, context):
        from . import export_xcd

        keywords = self.as_keywords(ignore=(
                                            "check_existing",
                                            "filter_glob",
                                            ))

        return export_xcd.save(self, context, **keywords)


def menu_func_export(self, context):
    self.layout.operator(ExportXCD.bl_idname,
                         text="Carbon Stage (.xcd)")


def register():
    bpy.utils.register_module(__name__)

    bpy.types.INFO_MT_file_export.append(menu_func_export)


def unregister():
    bpy.utils.unregister_module(__name__)

    bpy.types.INFO_MT_file_export.remove(menu_func_export)


if __name__ == "__main__":
    register()
