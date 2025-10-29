## packages
import argparse 
import configparser


## models
from core import dataset, utility, mongodb
from core.logger import Logger
from core.converter import converter

def load_args():
    # Create the argument parser
    parser = argparse.ArgumentParser(description="MetaBIM IFC Converter")

    # Add arguments
    parser.add_argument("--w", type=str, help="The guid of workspace", required=True)
    parser.add_argument("--p", type=str, help="The guid of project", required=True, default=0)
    parser.add_argument("--v", type=str, help="The guid of version", required=True, default=0)

    # Parse the arguments
    args = parser.parse_args()


    # Use the arguments
    Logger.info("=== Parsed Arguments ===")
    Logger.info(f"Workspace GUID : {args.w}")
    Logger.info(f"Project GUID   : {args.p}")
    Logger.info(f"Version GUID   : {args.v}")

    return {
        "workspace": args.w,
        "project": args.p,
        "version": args.v
    }



# Load configuration
def load_config(path="config.ini"):
    config = configparser.ConfigParser()
    config.read(path)

    # Extract database settings
    mongodb_uri = config["database"]["mongodb_uri"]
    database_name = config["database"]["database"]
    domain = config["database"]["domain"]

    # Extract general settings
    debug_mode = config.getboolean("settings", "debug", fallback=True)
    debug_filename = config.getboolean("settings", "debug_file", fallback=False)


    # print the result
    Logger.info("=== Loaded Configuration ===")
    Logger.info(f"MongoDB URI   : {mongodb_uri}")
    Logger.info(f"Database Name : {database_name}")
    Logger.info(f"Debug Mode    : {debug_mode}")

    return {
        "mongodb_uri": mongodb_uri,
        "database": database_name,
        "debug": debug_mode,
        "domain": domain,
    }

## Run the main function
def main():
    dataset.configuration   = load_config()
    dataset.argument        = load_args()

    ## start conversion process
    converter.convertion_process()


    
## Example Data
## workspce: fe4f891cbcd547458b1679f0c642fe9b
## project:  e5cb29b26e09465eb75cc6279bd1193b
## version:  e0dbc224baac4c9180e6c5a340fcf4a3

##  python .\ConverterCore.py --w fe4f891cbcd547458b1679f0c642fe9b --p e5cb29b26e09465eb75cc6279bd1193b --v e0dbc224baac4c9180e6c5a340fcf4a3

if __name__ == "__main__":
    main()