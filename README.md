- Unity : 2021.3.0f1
- ML-Agents : 2.0.1

**ML-Agents – Entraîner de vrais petits agents intelligents**
Bassem YAGOUB - Stage 2022 ViBE


Ce repository constitue le livrable Unity réalisé lors de mon stage.
Mon stage a pour but d’étudier les possibilités offertes par la librairie Unity nommée ML-Agents,
d’évaluer des cas d’usage pertinents, investiguer sur les besoins lors des projets IA chez
OCTO, et de concevoir une application Unity apportant une réponse à un besoin d’un cas
d’usage identifié.

Pour ce faire nous avons opté pour la *conduite autonome de véhicule.*

Vous retrouverez donc sur ce repository un environnement où des agents peuvent s'entraîner et être testés sur des routes variées.
Les véhicules doivent passer par des checkpoints à disposer sur les routes afin de les aider à apprendre avec une politique de récompense à déterminer.

Si vous n'avez pas de connaissance sur la librairie ML-Agents, je vous recommande vivement de lire la documentation très complète :
[Documentation ML-Agents](https://github.com/Unity-Technologies/ml-agents/tree/main/docs)

Les choses à savoir :
- Un lot d'assets pour créer vos propres routes est disponible dans le dossier *Assets/Prefab/*.
- De nombreux modèles entraînés avec différents algorithmes et différents capteurs sont disponibles dans le dossier *Assets/ML-Agents/Training Files/results/*
- Si vous voulez visualiser vos résultats d'entraînement, vous pouvez utiliser l'outil TensorBoard détaillé [ici](https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Using-Tensorboard.md)

```
.
├── Assets                     --> Dossier source où sont les fichiers ajoutés par l'utilisateur
│   ├── Materials              --> Contient les materials
│   ├── ML-Agents              --> Contient tous les fichiers relatifs à la librairie ML-Agents
│	│	├── Demos              --> Contient des demos pour entraîner des modèles utilisant de l'imitation learning
│	│	├── Timers             --> Contient des fichiers timers créés par ML-Agents
│	│	└── Training Files     --> Contient les fichiers de paramètres d'entraînement 
│	│		└── results        --> Contient les modèles entraînés
│	│			└── ...
│   ├── Prefabs                --> Contient les prefabs utilisés dans le projet
│	│	└── Kenney Racing Kit  --> Contient des assets pour créer des routes et un environnement
│   ├── Scenes                 --> Contient la scène utilisé pour le projet
│   ├── Scripts                --> Contient tous les fichiers de scripts
│   └── ...
├── Executables                --> Contient les executables et autres fichiers nécessaires aux exe
│   ├── Windows
│	│	├── Autonomous Car.exe --> Exécutable pour avoir un aperçu d'un des modèles sur une route test
│   │   └── ...
│   └── ...
└── ...
```