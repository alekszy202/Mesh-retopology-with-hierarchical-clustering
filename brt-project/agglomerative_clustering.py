import numpy as np
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
from sklearn.cluster import AgglomerativeClustering
from scipy.cluster.hierarchy import dendrogram

from IO import error_exit

def compute(data, n_clusters, distanceThreshold, affinity, linkage, show_charts):    
    if show_charts:
        fig = plt.figure()
        ax = Axes3D(fig)
        ax.scatter(data[:,0], data[:,1], data[:,2], s=300)
        ax.view_init(azim=200)
        plt.show()

    try:
        model = AgglomerativeClustering(n_clusters=n_clusters, distance_threshold=distanceThreshold, affinity=affinity, linkage=linkage, compute_distances=True)
        model.fit(data)
    except (ValueError, MemoryError) as ex:
        error_exit(ex)

    if show_charts:
        fig = plt.figure()
        ax = Axes3D(fig)
        ax.scatter(data[:,0], data[:,1], data[:,2], c=model.labels_, s=300)
        ax.view_init(azim=200)
        plt.show()

        plt.title("Hierarchical Clustering Dendrogram")
        # plot the top three levels of the dendrogram
        plot_dendrogram(model, truncate_mode="level", p=7)
        plt.xlabel("Number of points in node (or index of point if no parenthesis).")
        plt.show()

    print(f"Number of cluster found: {len(set(model.labels_))}")
    print(f"Cluster for each point: {model.labels_}")
    return (model.children_, len(set(model.labels_)))


def plot_dendrogram(model, **kwargs):
    # Create linkage matrix and then plot the dendrogram

    # create the counts of samples under each node
    counts = np.zeros(model.children_.shape[0])
    n_samples = len(model.labels_)
    for i, merge in enumerate(model.children_):
        current_count = 0
        for child_idx in merge:
            if child_idx < n_samples:
                current_count += 1  # leaf node
            else:
                current_count += counts[child_idx - n_samples]
        counts[i] = current_count

    linkage_matrix = np.column_stack([model.children_, model.distances_, counts]).astype(float)

    # Plot the corresponding dendrogram
    dendrogram(linkage_matrix, **kwargs)