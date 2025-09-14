using SaberTools.Common;
using System.IO;

// This file is part of Model Converter for Space Marine 2
// Copyright (C) 2025 Neo_Kesha
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// See <https://www.gnu.org/licenses/> for more details.

namespace SaberTools.USF
{
    public class Usf
    {
        public UsfScene scene { get; set; }
        public string sourcePath { get; set; }
        public string type { get; set; }
        public Ps options { get; set; }
        public int version { get; set; } = 257;
        public void Read(BinaryReader reader)
        {
            version = reader.ReadInt32();
            sourcePath = UsfString.Read(reader);
            type = UsfString.Read(reader);
            options = new Ps();
            options.Read(reader);
            scene = new UsfScene();
            scene.Read(reader);
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(version);
            UsfString.Write(writer, sourcePath);
            UsfString.Write(writer, type);
            options.Write(writer);
            scene.Write(writer);
        }

        public void Initialize(string actorRootName, bool isCharacter)
        {
            sourcePath = "";
            type = "tpl";
            options = GenerateOptions();
            scene = GenerateScene(actorRootName, isCharacter);
        }

        private Ps GenerateOptions()
        {
            var ps = new Ps();
            ps.str = "\r\noptions    =    {\r\n    geom    =    {\r\n        optimize_pc    =    TRUE\r\n        share_geometry    =    TRUE\r\n        build_skin_compound    =    TRUE\r\n        weld_vtx    =    0.001000\r\n        weld_uv    =    0.001000\r\n        export_decals    =    FALSE\r\n        subdivide_vis    =    FALSE\r\n        subdivide_edge    =    FALSE\r\n        subdivide_merge    =    FALSE\r\n        subdivide_merge_count    =    200\r\n        disable_tpl_edge    =    TRUE\r\n        dyn_cont    =    FALSE\r\n    }\r\n\tadvanced    =    {\r\n        disable_bulldozer    =    TRUE\r\n        disable_export    =    FALSE\r\n        disable_resource_manager    =    FALSE\r\n        disable_shadow_map_converter    =    FALSE\r\n        memory_file_size    =    96\r\n        optim_tpl    =    TRUE\r\n        dont_run_lsagen    =    TRUE\r\n        show_lsagen_wnd    =    FALSE\r\n        disable_scene_optimization    =    FALSE\r\n        disable_scene_group_optimization    =    FALSE\r\n        remove_degenerated_faces_havok    =    TRUE\r\n        export_mtl_in_text_format    =    FALSE\r\n        optimize_indices_for_tl_cache    =    TRUE\r\n        export_tpl_data    =    TRUE\r\n    }\r\n    compression    =    {\r\n        compress_verts_tpl    =    TRUE\r\n        compress_stat_verts    =    {\r\n            precision    =    0.002000\r\n            __type_id    =    \"yes\"\r\n        }\r\n        compress_normals    =    TRUE\r\n        compress_norm_to_4verts    =    TRUE\r\n        compress_texture    =    TRUE\r\n    }\r\n    sync_folder    =    FALSE\r\n}\r\n";
            return ps;
        }

        private UsfScene GenerateScene(string actorRootName, bool isCharacter)
        {
            var scene = new UsfScene();
            scene.Initialize(actorRootName, isCharacter);
            return scene;
        }
    }
}
