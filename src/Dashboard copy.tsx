import React, { useState, useEffect } from 'react';
import {
  Box, Heading, Text, Button, Stack, Flex, Grid, Table, Thead, Tbody, Tr, Th, Td, useDisclosure, Collapse
} from '@chakra-ui/react';
import { Modal, ModalOverlay, ModalContent, ModalHeader, ModalFooter, ModalBody, ModalCloseButton } from '@chakra-ui/react';
import { useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';

interface Enrollment {
  courseName: string;
  completionStatus: string;
  progress?: number;
}

interface Course {
  courseId: number;
  courseName: string;
  description: string;
  maxSeats: number;
  currentSeats: number;
  startDate: string;
  endDate: string;
}

interface Student {
  studentId: number;
  fullName: string;
  email: string;
  enrollementdate: string;
}

const Dashboard: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { fullName, userId, userType } = location.state || {}; // Extract userType here

  const [enrollments, setEnrollments] = useState<Enrollment[]>([]);
  const [availableCourses, setAvailableCourses] = useState<Course[]>([]);
  const [students, setStudents] = useState<Student[]>([]); // State to hold student list
  const [selectedStudentEnrollments, setSelectedStudentEnrollments] = useState<Enrollment[]>([]); // Enrollments for selected student
  const [selectedStudentName, setSelectedStudentName] = useState<string | null>(null); // Selected student's name

  const [selectedCourseDetails, setSelectedCourseDetails] = useState<Course | null>(null); // Selected course details
  const [isCoursesOpen, setIsCoursesOpen] = useState(false); // Collapsible state for the courses list

  const { isOpen, onOpen, onClose } = useDisclosure();  // Modal state control
  const [studentModalOpen, setStudentModalOpen] = useState(false);  // Modal for student enrollments
  const [courseModalOpen, setCourseModalOpen] = useState(false);  // Modal for course details
  const [isPreviousClassesOpen, setIsPreviousClassesOpen] = useState(false);  // Set to false initially
  const [isStudentsOpen, setIsStudentsOpen] = useState(false);  // Collapsible state for the student list
  const [modalMessage, setModalMessage] = useState<string | null>(null);  // Modal message state
  

  useEffect(() => {
    // Fetch courses regardless of the userType
    const fetchCourses = async () => {
      try {
        const response = await axios.get('http://localhost:5000/api/Course/ShowAllCourses');
        setAvailableCourses(response.data);
      } catch (error) {
        console.error('Error fetching courses:', error);
      }
    };
  
    if (userType === 1) {
      // Admin user: Fetch the list of students and courses
      const fetchStudents = async () => {
        try {
          const response = await axios.get('http://localhost:5000/api/Students/ShowAllStudents');
          setStudents(response.data);
        } catch (error) {
          console.error('Error fetching students:', error);
        }
      };
  
      fetchStudents();
      fetchCourses(); // Fetch courses for admin users
    } else if (userType === 2) {
      // Student user: Fetch enrollments and courses
      const fetchEnrollments = async () => {
        try {
          const response = await axios.get('http://localhost:5000/api/Enrollments/GetAllEnrollments');
          const studentEnrollments = response.data.data.filter((enrollment: any) => enrollment.studentName === fullName);
  
          const processedEnrollments = studentEnrollments.map((enrollment: any) => {
            let progress = 0;
            if (enrollment.completionStatus === "1") {
              progress = 100;
            } else {
              progress = Math.floor(Math.random() * (90 - 20 + 1)) + 20;
            }
            return {
              courseName: enrollment.courseName,
              completionStatus: enrollment.completionStatus,
              progress,
            };
          });
  
          setEnrollments(processedEnrollments);
        } catch (error) {
          console.error('Error fetching enrollments:', error);
        }
      };
  
      fetchEnrollments();
      fetchCourses(); // Fetch courses for student users
    }
  }, [fullName, userType]);

  const handleEnroll = async (courseName: string) => {
    try {
      // Log the request body for debugging purposes
      const requestBody = {
        studentName: fullName,
        courseName: courseName,
      };
  
      console.log('Request Body:', requestBody);
  
      // Make the API call to enroll the student in the course
      const response = await axios.post('http://localhost:5000/api/Enrollments/JoinCourse', requestBody);
  
      // Log the server response for debugging
      console.log('Server Response:', response.data);
  
      if (response.data && response.data.Message) {
        alert(response.data.Message);  // Show the message as an alert
      } else {
        alert('Successfully enrolled in the course.');  // Default success message
      }
  
      // Refetch the enrollments after enrolling in a course
      const fetchEnrollments = async () => {
        try {
          const response = await axios.get('http://localhost:5000/GetAllEnrollments');
          console.log('Enrollments:', response.data);
  
          const studentEnrollments = response.data.data.filter(
            (enrollment: any) => enrollment.studentName === fullName
          );
  
          const processedEnrollments = studentEnrollments.map((enrollment: any) => {
            let progress = 0;
            if (enrollment.completionStatus === "1") {
              progress = 100;
            } else {
              progress = Math.floor(Math.random() * (90 - 20 + 1)) + 20;
            }
            return {
              courseName: enrollment.courseName,
              completionStatus: enrollment.completionStatus,
              progress,
            };
          });
  
          setEnrollments(processedEnrollments);
        } catch (error) {
          console.error('Error refetching enrollments:', error);
          alert('Error refetching enrollments. Please try again.');
        }
      };
  
      fetchEnrollments();
    } catch (error: any) {
      // Improved error handling with detailed logging
      console.error('Error during enrollment:', error);
  
      if (error.response && error.response.data && error.response.data.Message) {
        alert(error.response.data.Message);  // Show the error message as an alert
      } else if (error.response && error.response.data) {
        alert(error.response.data);  // Show the raw error response data
      } else {
        alert('An unexpected error occurred. Please try again.');
      }
    }
  };
  

  // Fetch enrollments for a particular student
  const handleStudentClick = async (studentName: string) => {
    try {
      const response = await axios.get('http://localhost:5000/api/Enrollments/GetAllEnrollments');
      const studentEnrollments = response.data.data.filter((enrollment: any) => enrollment.studentName === studentName);

      const processedEnrollments = studentEnrollments.map((enrollment: any) => ({
        courseName: enrollment.courseName,
        completionStatus: enrollment.completionStatus,
        progress: enrollment.completionStatus === "1" ? 100 : Math.floor(Math.random() * (90 - 20 + 1)) + 20,
      }));

      setSelectedStudentEnrollments(processedEnrollments);
      setSelectedStudentName(studentName);
      setStudentModalOpen(true); // Open the modal
    } catch (error) {
      console.error('Error fetching student enrollments:', error);
    }
  };

  // Fetch course details when clicking on a course
  const handleCourseClick = async (courseId: number) => {
    try {
      const course = availableCourses.find(c => c.courseId === courseId);
      if (course) {
        setSelectedCourseDetails(course);
        setCourseModalOpen(true); // Open the modal
      }
    } catch (error) {
      console.error('Error fetching course details:', error);
    }
  };

  const handleLogout = () => {
    navigate('/');
  };

  const togglePreviousClasses = () => {
    setIsPreviousClassesOpen(!isPreviousClassesOpen);
  };

  const toggleStudents = () => {
    setIsStudentsOpen(!isStudentsOpen);
  };

  const toggleCourses = () => {
    setIsCoursesOpen(!isCoursesOpen);
  };

  // Split enrollments into active and previous
  const activeClasses = enrollments.filter((e) => e.completionStatus === '0');
  const previousClasses = enrollments.filter((e) => e.completionStatus === '1');

  const getProgressColor = (progress: number) => {
    if (progress === 100) return 'green.300';
    if (progress < 60) return 'red.300';
    if (progress < 90) return 'yellow.300';
    return 'teal.300';  // Default color
  };

  return (
    <Flex minHeight="100vh" width="100vw" justify="center" align="start" bg="gray.50" p={6}>
      <Box p={8} maxWidth="1200px" width="100%" borderWidth={1} borderRadius={8} boxShadow="lg" bg="white">
        <Heading as="h2" size="xl" mb={6} textAlign="center">
          {userType === 1 ? 'Admin Dashboard' : 'Student Dashboard'}
        </Heading>
        
        <Heading as="h6" size="l" mb={5} textAlign="center">
          User ID: {userId}, <i><b>{fullName}</b></i>
        </Heading>
         {/* Modal for showing enrollment success or error messages */}
         <Modal isOpen={isOpen} onClose={onClose}>
          <ModalOverlay />
          <ModalContent>
            <ModalHeader>Enrollment Status</ModalHeader>
            <ModalCloseButton />
            <ModalBody>
              <Text>{modalMessage}</Text>
            </ModalBody>
            <ModalFooter>
              <Button colorScheme="blue" onClick={onClose}>Close</Button>
            </ModalFooter>
          </ModalContent>
        </Modal>

        {userType === 1 ? (
          // Admin Dashboard Content
          <>
            {/* Collapsible Section for List of Students */}
            <Box>
              <Flex justifyContent="space-between" alignItems="center">
                <Heading as="h3" size="md" mb={4}>
                  List of Students
                </Heading>
                <Button size="sm" onClick={toggleStudents}>
                  {isStudentsOpen ? 'Hide' : 'Show'}
                </Button>
              </Flex>
              <Collapse in={isStudentsOpen}>
                <Flex overflowX="auto" maxHeight="400px">
                  <Table variant="simple" size="md" width="100%">
                    <Thead>
                      <Tr>
                        <Th position="sticky" top={0} bg="gray.100" width="25%" zIndex={1}>Student ID</Th>
                        <Th position="sticky" top={0} bg="gray.100" width="100%" zIndex={1}>Full Name</Th>
                        <Th position="sticky" top={0} bg="gray.100" width="100%" zIndex={1}>Email</Th>
                      </Tr>
                    </Thead>
                    <Tbody>
                      {students.length > 0 ? (
                        students.map((student) => (
                          <Tr key={student.studentId}>
                            <Td>{student.studentId}</Td>
                            {/* Make the student name clickable */}
                            <Td>
                              <Text
                                as="button"
                                color="blue.500"
                                onClick={() => handleStudentClick(student.fullName)}
                              >
                                {student.fullName}
                              </Text>
                            </Td>
                            <Td>{student.email}</Td>
                          </Tr>
                        ))
                      ) : (
                        <Tr>
                          <Td colSpan={5} textAlign="center">
                            No students found.
                          </Td>
                        </Tr>
                      )}
                    </Tbody>
                  </Table>
                </Flex>
              </Collapse>
            </Box>

            {/* Collapsible Section for List of Courses */}
            <Box mt={6}>
              <Flex justifyContent="space-between" alignItems="center">
                <Heading as="h3" size="md" mb={4}>
                  List of Courses
                </Heading>
                <Button size="sm" onClick={toggleCourses}>
                  {isCoursesOpen ? 'Hide' : 'Show'}
                </Button>
              </Flex>
              <Collapse in={isCoursesOpen}>
                <Flex overflowX="auto" maxHeight="400px">
                  <Table variant="simple" size="md" width="100%">
                    <Thead>
                      <Tr>
                        <Th position="sticky" top={0} bg="gray.100" width="25%" zIndex={1}>Course ID</Th>
                        <Th position="sticky" top={0} bg="gray.100" width="40%" zIndex={1}>Course Name</Th>
                        <Th position="sticky" top={0} bg="gray.100" width="100%" zIndex={1}>Description</Th>
                      </Tr>
                    </Thead>
                    <Tbody>
                      {availableCourses.length > 0 ? (
                        availableCourses.map((course) => (
                          <Tr key={course.courseId}>
                            <Td>{course.courseId}</Td>
                            {/* Make the course name clickable */}
                            <Td>
                              <Text
                                as="button"
                                color="blue.500"
                                onClick={() => handleCourseClick(course.courseId)}
                              >
                                {course.courseName}
                              </Text>
                            </Td>
                            <Td>{course.description}</Td>
                          </Tr>
                        ))
                      ) : (
                        <Tr>
                          <Td colSpan={5} textAlign="center">
                            No courses found.
                          </Td>
                        </Tr>
                      )}
                    </Tbody>
                  </Table>
                </Flex>
              </Collapse>
            </Box>
          </>
        ) : (
          // Student Dashboard Content
          <Stack spacing={6}>
            {/* Previous Classes Section - Collapsible */}
            <Box>
              <Flex justifyContent="space-between" alignItems="center">
                <Heading as="h3" size="md" mb={4}>
                  Completed Courses
                </Heading>
                <Button size="sm" onClick={togglePreviousClasses}>
                  {isPreviousClassesOpen ? 'Hide' : 'Show'}
                </Button>
              </Flex>
              <Collapse in={isPreviousClassesOpen}>
                <Flex overflowX="auto" direction="row" gap={4}>
                  {previousClasses.length > 0 ? (
                    previousClasses.map((enrollment, index) => (
                      <Box
                        key={index}
                        minWidth="250px"
                        p={4}
                        borderWidth={1}
                        borderRadius={8}
                        bg="green.300"
                        textAlign="center"
                      >
                        <Heading size="sm">{enrollment.courseName}</Heading>
                        <Text mt={2}>Progress: {enrollment.progress}%</Text>
                      </Box>
                    ))
                  ) : (
                    <Text>No previous classes found.</Text>
                  )}
                </Flex>
              </Collapse>
            </Box>

            {/* Active Classes Section */}
            <Box>
              <Heading as="h3" size="md" mb={4}>
                Active Courses
              </Heading>
              <Flex overflowX="auto" direction="row" gap={4}>
                {activeClasses.length > 0 ? (
                  activeClasses.map((enrollment, index) => (
                    <Box
                      key={index}
                      minWidth="250px"
                      p={4}
                      borderWidth={1}
                      borderRadius={8}
                      bg={getProgressColor(enrollment.progress!)}
                      textAlign="center"
                    >
                      <Heading size="sm">{enrollment.courseName}</Heading>
                      <Text mt={2}>Progress: {enrollment.progress}%</Text>
                    </Box>
                  ))
                ) : (
                  <Text>No active classes found.</Text>
                )}
              </Flex>
            </Box>

            {/* Available Classes Section */}
            <Box>
              <Heading as="h3" size="md" mb={4}>
                Available Classes
              </Heading>

              <Flex overflowX="auto" maxHeight="400px">
                <Table variant="simple" size="md" width="100%">
                  <Thead>
                    <Tr>
                      <Th position="sticky" top={0} bg="gray.100" zIndex={1}>Course Name</Th>
                      <Th position="sticky" top={0} bg="gray.100" zIndex={1}>Description</Th>
                      <Th position="sticky" top={0} bg="gray.100" zIndex={1}>Max Seats</Th>
                      <Th position="sticky" top={0} bg="gray.100" zIndex={1}>Current Seats</Th>
                      <Th position="sticky" top={0} bg="gray.100" zIndex={1}>Start Date</Th>
                      <Th position="sticky" top={0} bg="gray.100" zIndex={1}>End Date</Th>
                      <Th position="sticky" top={0} bg="gray.100" zIndex={1}>Enroll</Th>
                    </Tr>
                  </Thead>
                  <Tbody>
                    {availableCourses.length > 0 ? (
                      availableCourses.map((course) => (
                        <Tr key={course.courseId}>
                          <Td>{course.courseName}</Td>
                          <Td>{course.description}</Td>
                          <Td>{course.maxSeats}</Td>
                          <Td>{course.currentSeats}</Td>
                          <Td>{new Date(course.startDate).toLocaleDateString()}</Td>
                          <Td>{new Date(course.endDate).toLocaleDateString()}</Td>
                          <Td>
                            <Button colorScheme="teal" size="sm" onClick={() => handleEnroll(course.courseName)}>
                              Add
                            </Button>
                          </Td>
                        </Tr>
                      ))
                    ) : (
                      <Tr>
                        <Td colSpan={7} textAlign="center">
                          No available classes.
                        </Td>
                      </Tr>
                    )}
                  </Tbody>
                </Table>
              </Flex>
            </Box>
          </Stack>
        )}

        {/* Logout Button */}
        <Flex justifyContent="flex-end" mt={6}>
          <Button colorScheme="red" size="md" onClick={handleLogout}>
            Logout
          </Button>
        </Flex>

        {/* Modal for showing student enrollment details */}
        <Modal isOpen={studentModalOpen} onClose={() => setStudentModalOpen(false)}>
          <ModalOverlay />
          <ModalContent>
            <ModalHeader>{selectedStudentName}'s Enrollments</ModalHeader>
            <ModalCloseButton />
            <ModalBody>
              {selectedStudentEnrollments.length > 0 ? (
                <Stack spacing={4}>
                  {selectedStudentEnrollments.map((enrollment, index) => (
                    <Box key={index} p={4} borderWidth={1} borderRadius={8} bg={getProgressColor(enrollment.progress!)}>
                      <Heading size="sm">{enrollment.courseName}</Heading>
                      <Text>Completion Status: {enrollment.completionStatus === '1' ? 'Completed' : 'In Progress'}</Text>
                      <Text>Progress: {enrollment.progress}%</Text>
                    </Box>
                  ))}
                </Stack>
              ) : (
                <Text>No enrollments found for this student.</Text>
              )}
            </ModalBody>
            <ModalFooter>
              <Button colorScheme="blue" mr={3} onClick={() => setStudentModalOpen(false)}>
                Close
              </Button>
            </ModalFooter>
          </ModalContent>
        </Modal>

        {/* Modal for showing course details */}
        <Modal isOpen={courseModalOpen} onClose={() => setCourseModalOpen(false)}>
          <ModalOverlay />
          <ModalContent>
            <ModalHeader>{selectedCourseDetails?.courseName} Details</ModalHeader>
            <ModalCloseButton />
            <ModalBody>
              {selectedCourseDetails ? (
                <Stack spacing={4}>
                  <Text><strong>Description:</strong> {selectedCourseDetails.description}</Text>
                  <Text><strong>Max Seats:</strong> {selectedCourseDetails.maxSeats}</Text>
                  <Text><strong>Current Seats:</strong> {selectedCourseDetails.currentSeats}</Text>
                  <Text><strong>Start Date:</strong> {new Date(selectedCourseDetails.startDate).toLocaleDateString()}</Text>
                  <Text><strong>End Date:</strong> {new Date(selectedCourseDetails.endDate).toLocaleDateString()}</Text>
                </Stack>
              ) : (
                <Text>No details available for this course.</Text>
              )}
            </ModalBody>
            <ModalFooter>
              <Button colorScheme="blue" mr={3} onClick={() => setCourseModalOpen(false)}>
                Close
              </Button>
            </ModalFooter>
          </ModalContent>
        </Modal>
      </Box>
    </Flex>
  );
};

export default Dashboard;
