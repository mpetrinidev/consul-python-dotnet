"""
Runs the service on your local machine.

Usage:
    ./server.py run [options]
    ./server.py --help

Options:
    -h --help               Show this text.
    --host <host>           Host that holds the server.
    --port <port>           Port where the server listens for request.
    --host-consul <host>    Host that holds consul server.
    --port-consul <number>  Port where the consul server listens.
"""
import uuid
from asyncio import sleep

from consul import Check
from flask import Flask, Response
from docopt import docopt
import consul

app = Flask(__name__)

_opts = docopt(__doc__)

_HOST = _opts['--host']
_PORT = int(_opts['--port'])
_HOST_CONSUL = _opts['--host-consul']
_PORT_CONSUL = int(_opts['--port-consul'])

c = consul.Consul(host=_HOST_CONSUL, port=_PORT_CONSUL, dc="dc1")


@app.route('/', methods=['GET'])
def home():
    return Response(status=200, response={'value': 'Ok'})


@app.route('/health', methods=['GET'])
def health():
    # check dependencies
    return Response(status=200)


if __name__ == '__main__':
    http_addr = f'http://{_HOST}:{_PORT}/health'
    print(http_addr)

    while True:
        try:
            c.agent.service.register(name="py-service",
                                     service_id=str(uuid.uuid4()),
                                     address=_HOST,
                                     port=_PORT,
                                     check=Check.http(http_addr, '1s', timeout='5s'))
            break
        except (ConnectionError, consul.ConsulException) as e:
            print(f'Reconnecting: {str(e)}')
            sleep(0.5)

    app.run(host=_HOST, port=_PORT)
