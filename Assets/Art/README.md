# PedaLi — Free 3D Nature Asset Kit

This file is the asset acquisition and import plan for PedaLi's three environment biomes. The packs below are external downloads; they are **not bundled in this repository**.

## Download sources and licenses

| Pack | Best use in PedaLi | License / notes | Official link |
|---|---|---|---|
| Quaternius — Stylized Nature MegaKit | Main stylized foliage, plants, rocks and trees across all biomes | CC0; free standard download contains a subset of the full pack. FBX, OBJ and glTF. The complete Source version with prebuilt Unity shaders is not required. | https://quaternius.com/packs/stylizednaturemegakit.html |
| Quaternius — Ultimate Stylized Nature Pack | Alternative/extra trees, plants, rocks and seamless nature textures | CC0; 60+ assets in FBX, OBJ, glTF and Blend. | https://quaternius.com/packs/ultimatestylizednature.html |
| Kenney — Nature Kit | Lightweight blockout props and simple foliage/rocks | CC0; personal and commercial use allowed. | https://kenney.nl/assets/nature-kit |
| EmbersStorm — Free Nature Pack (Unity Asset Store) | Seasonal trees, summer/snow pines, bushes, grass and rocks; especially useful for mountain and snow variants | Free; Unity Asset Store Standard EULA, not CC0. Page lists Built-in, URP and HDRP support and Unity 2021.3 LTS+. Recheck the current listing before import. | https://assetstore.unity.com/packages/3d/environments/embersstorm-free-nature-pack-369910 |
| Free Stylized Nature Environment (Unity Asset Store) | Small sample environment, trees, grass, flowers, mountain, skybox and ground texture | Free; Unity Asset Store Standard EULA, not CC0. | https://assetstore.unity.com/packages/3d/environments/free-stylized-nature-environment-96371 |
| Poly Haven — Snow 03 | Snow surface material for cliffs and ledges | CC0; texture is a material source, not a ready 3D environment. Reduce texture resolution for mobile. | https://polyhaven.com/a/snow_03 |
| Poly Haven — Nature models | Optional individual trees and nature props | CC0; select only models needed to keep the Android build small. | https://polyhaven.com/models/nature/trees |

### License rules

- CC0 packs can be used in personal and commercial projects without required attribution, although crediting creators is good practice.
- Unity Asset Store free packages are still governed by the Asset Store EULA. Do not redistribute their raw package files outside the terms of that license.
- Keep each downloaded pack's original license/readme file in the local Unity project.
- Prices and package contents can change; verify the linked publisher page at download time.

## Biome assignment

### Jungle
- Main: Quaternius Stylized Nature MegaKit.
- Fill gaps: Kenney Nature Kit.
- Select broad-leaf plants, bushes, dense trees and mossy/rounded rocks where available.
- Palette target: deep green shadows, warm green foliage, turquoise distant haze.

### Mountains
- Main: Quaternius nature models + EmbersStorm rocks and pine trees.
- Use rocks as climbing ledges; keep large cliff silhouettes simple.
- Palette target: muted slate, warm grey-brown rock, evergreen trees and blue atmospheric distance.

### Snow Peaks
- Main: EmbersStorm snow variants + Quaternius rocks.
- Add Poly Haven Snow 03 to suitable ledge surfaces; use a simple snow-colored material on low-detail objects.
- Palette target: off-white snow, pale blue shadow, cool grey rock and distant blue haze.

## Unity import and Android performance checklist

1. Prefer **URP** if the project is already configured for it. Do not switch render pipelines in an existing project without checking materials and scenes.
2. Import FBX/glTF/OBJ models from the downloaded packs. Check scale, pivot, normals, materials and collider needs.
3. For small background props, disable real-time shadows. Keep shadow-casting trees near the play area only.
4. Use GPU instancing where compatible; share materials and avoid creating one unique material per tree.
5. Start with 512–1024 px textures for most props; test 2048 px only for large, close-up surfaces. Use ASTC on supported Android devices or the project's tested texture compression format.
6. Prefer simple colliders for climbable ledges. Decorative foliage should usually have no collider.
7. Use LODs or simplified distant meshes when available. Avoid excessive transparent leaf materials and overdraw.
8. Keep distant mountains in the existing parallax background system rather than placing hundreds of full-detail meshes in the scene.
9. Test on a real target Android device; asset-store compatibility labels do not guarantee a stable frame rate on every phone.

## Suggested folder layout

After downloading and checking each package, keep its original structure or organize imported assets under:

```text
Assets/
  Art/
    Nature/
      Quaternius/
      Kenney/
      EmbersStorm/
      PolyHaven/
    Materials/
      Jungle/
      Mountains/
      SnowPeaks/
    Prefabs/
      Jungle/
      Mountains/
      SnowPeaks/
```

Do not commit large third-party packages to the PedaLi source repository unless you have checked redistribution rights. This manifest intentionally contains links and setup guidance rather than copies of third-party asset files.
