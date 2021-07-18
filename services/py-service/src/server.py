import os
import uuid
from asyncio import sleep
from urllib.parse import urlparse, urljoin

import consul
from consul import Check
from flask import Flask, Response

app = Flask(__name__)

api_url = os.environ['API_URL']
api_url_parse = urlparse(api_url)

consul_url_parse = urlparse(os.environ['CONSUL_URL'])

c = consul.Consul(host=consul_url_parse.hostname, port=consul_url_parse.port)


@app.route('/', methods=['GET'])
def home():
    return Response(status=200, response={'value': 'Ok'})


@app.route('/health', methods=['GET'])
def health():
    # check dependencies
    return Response(status=200)


if __name__ == '__main__':
    http_addr = urljoin(api_url, 'health')

    while True:
        try:
            c.agent.service.register(name="py-service",
                                     service_id=str(uuid.uuid4()),
                                     address=api_url_parse.hostname,
                                     port=api_url_parse.port,
                                     check=Check.http(http_addr, '1s', timeout='5s'))
            break
        except (ConnectionError, consul.ConsulException) as e:
            print(f'Reconnecting: {str(e)}')
            sleep(0.5)

    app.run(host=api_url_parse.hostname, port=api_url_parse.port)
