from setuptools import find_packages, setup
setup(
    name="linql_client",
    packages=find_packages(include=["linql_client"]),
    version="1.0.0a10",
    description="Python linql client",
    author="Kris Sodroski",
    license="Proprietary",
    install_requires=[
        "functional"
    ],
    setup_requires=['pytest-runner'],
    tests_require=['pytest==4.4.1'],
    test_suite='tests',
)