import numpy as np
import IO
import agglomerative_clustering
from paths import (INPUT_PATH, OUTPUT_PATH, PARAM_PATH)

def main():
    data = np.genfromtxt(INPUT_PATH, delimiter=',')
    
    n_clusters, distanceThreshold, affinity, linkage, show_charts = IO.read_parameters_file(PARAM_PATH)
    children_data, cluster_count = agglomerative_clustering.compute(data, n_clusters, distanceThreshold, affinity, linkage, show_charts)
    IO.save_result_to_file(OUTPUT_PATH, children_data, cluster_count)

if __name__ == "__main__":
    main()
    IO.succeeded_exit()