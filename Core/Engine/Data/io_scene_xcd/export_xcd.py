# ##### BEGIN LICENSE BLOCK #####
#
#  @PG, Carbon
#
# ##### END LICENSE BLOCK #####

# -------------------------------------------------------------------------
# Imports
# -------------------------------------------------------------------------
import math
import os
import bpy
import mathutils
import gzip
import bpy_extras

from bpy_extras.io_utils import unique_name, create_derived_objects, free_derived_objects
from xml.sax.saxutils import quoteattr, escape

print("Version 0.1")

class XCDExporter:
    # -------------------------------------------------------------------------
    # Globals
    # -------------------------------------------------------------------------
    _uuidCacheObjects = {}
    _uuidCacheLights = {}
    _uuidCacheView = {}
    _uuidCacheWorld = {}
    
    _file = None
    _filePath = None
    _fileName = None
    _fileWriter = None
    _globalMatrix = None

    _debugIndent = 0
    
    # -------------------------------------------------------------------------
    # Constructor
    # -------------------------------------------------------------------------
    def __init__(self, filePath, globalMatrix):
        self._file = open(filePath, 'w', encoding='utf-8')
        self._fileName = self._file.name
        self._filePath = quoteattr(os.path.basename(self._fileName));        
        self._fileWriter = self._file.write
        self._globalMatrix = globalMatrix
        
    # -----------------------------------------------------------------------------
    # Main export function
    # -----------------------------------------------------------------------------
    def Export(self, scene, useSelection=True):
        # tag un-exported IDs
        bpy.data.meshes.tag(False)
        bpy.data.materials.tag(False)
        bpy.data.images.tag(False)
    
        self._p('Info: starting XCD export to %r...' % self._fileName)
        self._WriteHeader()
        self._WriteFog(scene.world)
    
        if useSelection:
            objects = [obj for obj in scene.objects if obj.is_visible(scene) and obj.select]
        else:
            objects = [obj for obj in scene.objects if obj.is_visible(scene)]
    
        hierarchy = self._BuildHierarchy(objects)
        self._p(hierarchy)
        for object, children in hierarchy:
            self._ExportObject(scene, object, children)
    
        self._WriteFooter()
        self._p('Info: finished XCD export')
        
    # -------------------------------------------------------------------------
    # Misc helper functions
    # -------------------------------------------------------------------------
    def _ClampColor(self, color):
        return tuple([max(min(c, 1.0), 0.0) for c in color])
    
    def _MatrixNegateZ(self, matrix):
        return (matrix.to_3x3() * mathutils.Vector((0.0, 0.0, -1.0))).normalized()[:]
    
    def _GzipOpenUtf8(self, filePath, mode):
        """Workaround for py3k only allowing binary gzip writing"""
        
        # need to investigate encoding
        file = gzip.open(filePath, mode)
        write_real = file.write
    
        def write_wrap(data):
            return write_real(data.encode("utf-8"))
        
        file.write = write_wrap
        return file
    
    
    def _BuildHierarchy(self, objects):
        """ returns parent child relationships, skipping """
        objectSet = set(objects)
        parentLookup = {}
    
        def testParent(parent):
            while (parent is not None) and (parent not in objectSet):
                parent = parent.parent
            return parent
    
        for obj in objects:
            self._p('Info: TestParent: %s' % obj.name)
            parentLookup.setdefault(testParent(obj.parent), []).append((obj, []))
    
        for parent, children in parentLookup.items():
            for obj, subchildren in children:
                subchildren[:] = parentLookup.get(obj, [])
    
        return parentLookup.get(None, [])
    
    def _Clean(self, text):
        if not text:
            text = "None"
            
        # no digit start
        if text[0] in "1234567890+-":
            text = "_" + text
            
        return text.translate({  # control characters 0x0-0x1f
                            # 0x00: "_",
                              0x01: "_",
                              0x02: "_",
                              0x03: "_",
                              0x04: "_",
                              0x05: "_",
                              0x06: "_",
                              0x07: "_",
                              0x08: "_",
                              0x09: "_",
                              0x0a: "_",
                              0x0b: "_",
                              0x0c: "_",
                              0x0d: "_",
                              0x0e: "_",
                              0x0f: "_",
                              0x10: "_",
                              0x11: "_",
                              0x12: "_",
                              0x13: "_",
                              0x14: "_",
                              0x15: "_",
                              0x16: "_",
                              0x17: "_",
                              0x18: "_",
                              0x19: "_",
                              0x1a: "_",
                              0x1b: "_",
                              0x1c: "_",
                              0x1d: "_",
                              0x1e: "_",
                              0x1f: "_",
    
                              0x7f: "_",  # 127
    
                              0x20: "_",  # space
                              0x22: "_",  # "
                              0x27: "_",  # '
                              0x23: "_",  # #
                              0x2c: "_",  # ,
                              0x2e: "_",  # .
                              0x5b: "_",  # [
                              0x5d: "_",  # ]
                              0x5c: "_",  # \
                              0x7b: "_",  # {
                              0x7d: "_",  # }
                              })

    def _GetDebugIndent(self):
        indent = ""
        for i in range(0, self._debugIndent):
            indent = indent + " -- "
        return indent

    def _dump(self, obj):
        for attr in dir(obj):
            try:
                self._p("obj.%s = %s" % (attr, getattr(obj, attr)))
            except:
                self._p("Could not get attribute for %s " % attr)
    
    def _p(self, m):
        print("%s %s" % (self._GetDebugIndent(), m))
    
    # -------------------------------------------------------------------------
    # File Writing Functions
    # -------------------------------------------------------------------------
    def _QuaternionToData(self, quaternion):
        return "%f %f %f %f" % (quaternion.x, quaternion.y, quaternion.z, quaternion.w)

    def _WriteHeader(self):
        blenderVersion = quoteattr('Blender %s' % bpy.app.version_string)
    
        self._fileWriter('<?xml version="1.0" encoding="UTF-8"?><xcd version="1.0">')
        self._fileWriter('<head>')
        self._fileWriter('<meta name="filename" content=%s />' % self._filePath)
        self._fileWriter('<meta name="generator" content=%s />' % blenderVersion)
        self._fileWriter('</head><scene>')
    
    def _WriteFooter(self):
        self._fileWriter('</scene></xcd>')
    
    def _WriteCamera(self, obj):
        self._p("Writing camera %s" % obj.name)
        
        id = quoteattr(unique_name(obj, obj.name, self._uuidCacheView, clean_func=self._Clean, sep="_"))
        if obj.rotation_mode != "QUATERNION":
            self._p("ERROR: Rotation mode has to be quaternion but was " + obj.rotation_mode)
            return
        location, rotation, scale = obj.matrix_local.decompose()
        self._p(rotation)
        self._fileWriter('<camera id=%s' % id)
        self._fileWriter(' fov="%.3f"' % obj.data.angle)
        self._fileWriter('>')
        
        self._fileWriter('<position>%3.2f %3.2f %3.2f</position>' % location[:])
        self._fileWriter('<rotation>%s</rotation>' % self._QuaternionToData(rotation))
        
        self._WriteLayers(obj.layers)
        self._WriteCustomProperties(obj)
        
        self._fileWriter('</camera>')
                
    def _BeginStageElement(self, obj):
        id = quoteattr(unique_name(obj, obj.name, self._uuidCacheObjects, clean_func=self._Clean, sep="_"))
        self._p("Writing stage element %s as %s" % (obj.name, id))
        
        location, rotation, scale = object.matrix_local.decompose()
        rotation = obj.rotation_quaternion
                
        link = obj.library.filepath.replace("//", "").replace(".blend", ".dae");
        self._fileWriter('<element id=%s link="%s"' % (id, link))
        self._fileWriter('>')
        
        self._fileWriter('<translation>%.6f %.6f %.6f</translation>' % location[:])
        self._fileWriter('<rotation>%s</rotation>' % self._QuaternionToData(rotation))
        self._fileWriter('<scale>%.6f %.6f %.6f</scale>' % scale[:])
        
        self._WriteBoundingBox(obj.bound_box)
        self._WriteLayers(obj.layers)
        self._WriteCustomProperties(obj)

    def _EndStageElement(self, obj):
        self._fileWriter('</element>')
    
    def _WriteFog(self, world):
        self._p("Writing fog")
        
        if world:
            mtype = world.mist_settings.falloff
            mparam = world.mist_settings
        else:
            return
    
        if mparam.use_mist:
            self._fileWriter('<fog type="%s"' % ('LINEAR' if (mtype == 'LINEAR') else 'EXPONENTIAL'))
            self._fileWriter(' depth="%.3f"' % mparam.depth)
            self._fileWriter('>')
            
            self._fileWriter('<color>%.3f %.3f %.3f</color>' % self._ClampColor(world.horizon_color))
            
            self._fileWriter('</fog>')
        else:
            return
    
    def _WriteSpotLight(self, obj):
        self._p("Writing spot light %s" % obj.name)
        lamp = obj.data
        
        id = quoteattr(unique_name(obj, obj.name, self._uuidCacheLights, clean_func=self._Clean, sep="_"))
        
        # compute cutoff and beam width
        intensity = min(lamp.energy / 1.75, 1.0)
        spotSize = lamp.spot_size * 0.37
        angle = spotSize * 1.3
        orientation = self._MatrixNegateZ(matrix)
        location = obj.matrix_local.to_translation()[:]
        radius = lamp.distance * math.cos(spotSize)
    
        self._fileWriter('<light type="Spot" id=%s' % id)
        self._fileWriter(' radius="%.4f"' % radius)
        self._fileWriter(' intensity="%.4f"' % intensity)        
        self._fileWriter(' spotsize="%.4f"' % spotSize)
        self._fileWriter(' angle="%.4f"' % angle)        
        self._fileWriter('>')
        
        self._fileWriter('<color>%.4f %.4f %.4f</color>' % self._ClampColor(lamp.color))
        self._fileWriter('<direction>%.4f %.4f %.4f</direction>' % orientation)
        self._fileWriter('<location>%.4f %.4f %.4f</location>' % location)
        
        self._WriteLayers(obj.layers)
        self._WriteCustomProperties(obj)
        
        self._fileWriter('</light>')
        
    
    def _WriteDirectionalLight(self, obj):
        self._p("Writing directional light %s" % obj.name)
        lamp = obj.data
        
        id = quoteattr(unique_name(obj, obj.name, self._uuidCacheLights, clean_func=self._Clean, sep="_"))
        
        intensity = min(lamp.energy / 1.75, 1.0)
        orientation = self._MatrixNegateZ(matrix)
    
        self._fileWriter('<light type="Directional" id=%s' % id)
        self._fileWriter(' intensity="%.4f"' % intensity)        
        self._fileWriter('>')
        
        self._fileWriter('<color>%.4f %.4f %.4f</color>' % self._ClampColor(lamp.color))
        self._fileWriter('<direction>%.4f %.4f %.4f</direction>' % orientation)
        
        self._WriteLayers(obj.layers)
        self._WriteCustomProperties(obj)
        
        self._fileWriter('</light>')
    
    def _WritePointLight(self, obj):
        self._p("Writing point light %s" % obj.name)
        lamp = obj.data
        
        id = quoteattr(unique_name(obj, obj.name, self._uuidCacheLights, clean_func=self._Clean, sep="_"))

        intensity = min(lamp.energy / 1.75, 1.0)
        location = obj.matrix_local.to_translation()[:]
    
        self._fileWriter('<light type="Point" id=%s' % id)     
        self._fileWriter(' intensity="%.4f"' % intensity)
        self._fileWriter(' radius="%.4f"' % lamp.distance)        
        self._fileWriter('>')
        
        self._fileWriter('<color>%.4f %.4f %.4f</color>' % self._ClampColor(lamp.color))
        self._fileWriter('<location>%.4f %.4f %.4f</location>' % location)
        
        self._WriteLayers(obj.layers)
        self._WriteCustomProperties(obj)
        
        self._fileWriter('</light>')
        
    def _WriteValueArray(self, values):
        needSpace = False
        for entry in values:
            if needSpace:
                self._fileWriter(' ')
            self._fileWriter(str(entry))
            needSpace = True
            
    def _WriteBoolValueArray(self, values):
        needSpace = False
        for entry in values:
            if needSpace:
                self._fileWriter(' ')
            if entry:
                self._fileWriter(str(1))
            else:
                self._fileWriter(str(0))
            needSpace = True
            
    def _WriteBoundingBox(self, boundingBox):
        self._fileWriter('<boundingBox>')
        for element in boundingBox:
            self._fileWriter('<point>')
            self._WriteValueArray(element)
            self._fileWriter('</point>')
        self._fileWriter('</boundingBox>')
            
    def _WriteLayers(self, layerInfo):
        self._fileWriter('<layers>')
        self._WriteBoolValueArray(layerInfo)
        self._fileWriter('</layers>')
        
    def _WriteCustomProperty(self, name, property):
        self._fileWriter('<property id="%s"' % name)
        if isinstance(property, float):
            self._fileWriter(' type="Float" Value="%s"' % property.real)
        elif isinstance(property, int):
            self._fileWriter(' type="Int" Value="%s"' % property)
        elif isinstance(property, str):
            self._fileWriter(' type="String" Value="%s"' % property)
        else:
            self._p("Uknown type for custom property %s" % name)
        self._fileWriter('/>')
        
    def _WriteCustomProperties(self, hash):
        self._fileWriter('<customproperties>')
        for key in hash.keys():
            if key[0] == "_":
                continue
            self._WriteCustomProperty(key, hash[key])
        self._fileWriter('</customproperties>')
        
    def _GetCustomPropertyValue(self, hash, property):
        for key in hash.keys():
            if key[0] == "_" or key[0] != property:
                continue
            return hash[key]
        
    # -------------------------------------------------------------------------
    # Export Object Hierarchy (recursively called)
    # -------------------------------------------------------------------------
    def _ExportObject(self, scene, object, children):        
        self._p("-> Exporting %s" % object.name)
        self._p(" ROT: %s" % object.rotation_quaternion)
        objectType = object.type
        
        if objectType == 'CAMERA':
            self._WriteCamera(object)
                
        elif objectType == 'MESH':
            self._p("Ignoring mesh %s, currently not supported by xcd" % object.name)
            return

        elif objectType == 'LAMP':
            data = object.data
            type = data.type
            if type == 'POINT':
                self._WritePointLight(object)
            elif type == 'SPOT':
                self._WriteSpotLight(object)
            elif type == 'SUN':
                self._WriteDirectionalLight(object)
            else:
                self._WriteDirectionalLight(object)
        else:
            self._ExportDerived(scene, object)
                           
        for child, objectChildren in children:
            self._p(" CHILD: %s" % child.name)
            self._debugIndent = self._debugIndent + 1
            self._ExportObject(scene, child, objectChildren)
            self._debugIndent = self._debugIndent - 1

    def _ExportDerived(self, scene, object):
        free, derived = create_derived_objects(scene, object)
        if derived:
            prefabNodes = {}
            for derivedObject, derivedMatrix in derived:
                if derivedObject == object:
                    self._p("Derived is same!")
                    continue
                if derivedObject.library.filepath not in prefabNodes then:
                    prefabNodes[derivedObject.library.filepath] = {}
                self._p(" DERIV: %s %s %s" % (derivedObject.name, derivedObject.library.filepath, derivedObject.type))
                self._debugIndent = self._debugIndent + 1
                #self._ExportObject(scene, derivedObject, derivedObject.matrix_local, children)
                self._debugIndent = self._debugIndent - 1
        if free:
            free_derived_objects(object)
        

##########################################################
# Callbacks, needed before Main
##########################################################
def save(operator, context, filepath="", use_selection=False, global_matrix=None):
    bpy.path.ensure_ext(filepath, '.xcd')

    if bpy.ops.object.mode_set.poll():
        bpy.ops.object.mode_set(mode='OBJECT')
    
    if global_matrix is None:
        global_matrix = mathutils.Matrix()
        
    exporter = XCDExporter(filepath, global_matrix)
    exporter.Export(context.scene, useSelection=use_selection)

    return {'FINISHED'}
