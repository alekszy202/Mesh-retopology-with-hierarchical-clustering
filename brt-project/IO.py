import sys
import json

def save_result_to_file(path, children_data, cluster_count):
    print(f'Saving results to {path}')
    with open(fr'{path}', 'w') as f:
        f.write(f'{cluster_count}\n')

        counter = 0
        for child in children_data:
            if counter < len(children_data) - 1:
                f.write(f'{str(child[0])}, {str(child[1])}\n')
            else:
                f.write(f'{str(child[0])}, {str(child[1])}')
            counter += 1


def read_parameters_file(path):
    print(f'Reading parameters file: {path}')

    try:
        with open(fr'{path}', 'r') as f:
            parameter_data = json.load(f)
    except EnvironmentError:
        error_exit(f'Unable to open file {path}')

    try:
        n_clusters = parameter_data['nClusters']
        distanceThreshold = parameter_data['distanceThreshold']
        affinity = parameter_data['affinity']
        linkage = parameter_data['linkage']
        show_charts = parameter_data['showCharts']
    except KeyError as err:
        error_exit(f'Parameter {err} missing in file {path}')

    print(f'Finished reading parameters\n')
    return (n_clusters, distanceThreshold, affinity, linkage, show_charts)


def succeeded_exit():
    print('SUCCEEDED')

def error_exit(error_info):
    print('ERROR')
    print(f'Error occured: {error_info}')
    sys.exit()