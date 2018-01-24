﻿using System;
using Retyped;

using static Retyped.es5;
using static Retyped.dom;
using static Retyped.three;

using Math = Retyped.es5.Math;

namespace ThreeJsDemo
{
    /// <summary>
    /// See the original sample here:
    /// https://threejs.org/examples/#webgl_morphtargets_horse
    /// https://github.com/mrdoob/three.js/blob/da2936540a48774b043a1e617bddbdce5102e417/examples/webgl_morphtargets_horse.html
    /// </summary>
    public class App
    {
        private static PerspectiveCamera camera;
        private static Scene scene;
        private static AnimationMixer mixer;
        private static WebGLRenderer renderer;
        private static double theta;
        private static double radius = 600;
        private static double prevTime;
        private static Vector3 cameraTarget;

        public static void Main()
        {
            Init();
            animate();
        }

        public static void Init()
        {
            var container = document.createElement("div");
            document.body.appendChild(container);

            //
            camera = new PerspectiveCamera(50, window.innerWidth / window.innerHeight, 1, 10000);
            camera.position.y = 300;
            cameraTarget = new Vector3(0, 150, 0);

            scene = new Scene();
            scene.background = new Color(0xf0f0f0);
           
            //
            var light = new DirectionalLight(0xefefff, 1.5);
            light.position.set(1, 1, 1).normalize();
            scene.add(light);

            light = new DirectionalLight(0xffefef, 1.5);
            light.position.set(-1, -1, -1).normalize();
            scene.add(light);

            var loader = new JSONLoader();
            loader.load("https://raw.githubusercontent.com/mrdoob/three.js/master/examples/models/animated/horse.js", (geometry, materials) =>
            {
                var mesh = new Mesh(geometry, new MeshLambertMaterial(new MeshLambertMaterialParameters
                {
                    vertexColors = FaceColors,
                    morphTargets = true
                }));

                mesh.scale.set(1.5, 1.5, 1.5);
                scene.add(mesh);

                mixer = new AnimationMixer(mesh);

                var clip = AnimationClip.CreateFromMorphTargetSequence("gallop", geometry.morphTargets, 30, false);
                mixer.clipAction(clip).setDuration(1).play();
            });
           
            //
            renderer = new WebGLRenderer();
            renderer.setPixelRatio(window.devicePixelRatio);
            renderer.setSize(window.innerWidth, window.innerHeight);
            container.appendChild(renderer.domElement);
          
            //
            window.addEventListener("resize", e => onWindowResize(), false);
        }

        public static void onWindowResize()
        {
            camera.aspect = window.innerWidth / window.innerHeight;
            camera.updateProjectionMatrix();
            renderer.setSize(window.innerWidth, window.innerHeight);
        }

        public static void animate()
        {
            requestAnimationFrame(new Action(animate).As<FrameRequestCallback>());
            render();
        }

        public static void render()
        {
            theta += 0.1;
            camera.position.x = radius * Math.sin(three.Math.degToRad(theta));
            camera.position.z = radius * Math.cos(three.Math.degToRad(theta));

            camera.lookAt(cameraTarget);

            if (mixer != null)
            {
                var time = Date.now();
                mixer.update((time - prevTime) * 0.001);
                prevTime = time;
            }
            renderer.render(scene, camera);
        }
    }
}